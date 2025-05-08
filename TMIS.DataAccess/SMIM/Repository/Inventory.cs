using Dapper;
using Microsoft.AspNetCore.Http;
using System.Text;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Inventory(IDatabaseConnectionSys dbConnection,
        ISMIMLogdb iSMIMLogdb,
        ISessionHelper sessionHelper,
        IUserControls userControls) : IInventory
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IUserControls _userControls = userControls;

        public async Task<McCreateVM> LoadInventoryDropDowns(int? id)
        {
            var oMcCreateVM = new McCreateVM();
            if (id == null)
            {
                oMcCreateVM = new McCreateVM
                {
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels"),
                    GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups"),
                    LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations"),
                    OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits"),
                    McInventory = new McInventory()
                };
            }
            else
            {
                oMcCreateVM = new McCreateVM
                {
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels"),
                    GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups"),
                    LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations"),
                    OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits"),
                    McInventory = await GetMcInventoryByIdAsync(id)
                };
            }         

            return oMcCreateVM;
        }

        public async Task<McCreatedRnVM> LoadRentInventoryDropDowns(int? id)
        {
            var oMcCreatedRnVM = new McCreatedRnVM();

            if (id == null)
            {
                oMcCreatedRnVM = new McCreatedRnVM
                {
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels"),
                    GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups"),
                    LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations"),
                    OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits"),
                    SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineSuppliers"),
                    CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MdCostMethods"),
                    McInventory = new McInventory()
                };
            }
            else 
            {
                oMcCreatedRnVM = new McCreatedRnVM
                {
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels"),
                    GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups"),
                    LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations"),
                    OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits"),
                    SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineSuppliers"),
                    CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MdCostMethods"),

                    McInventory = await GetRentMcInventoryByIdAsync(id)
                };
            }
            return oMcCreatedRnVM;
        }

        public async Task<MachinesVM> GetList()
        {
            var sql = @"
            SELECT Id ,QrCode, SerialNo, MachineBrand, MachineType FROM VwMcInventory WHERE IsOwned = 1 AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;
            SELECT Id ,QrCode, SerialNo, DateBorrow, MachineType FROM VwMcInventory WHERE IsOwned = 0 AND CurrentUnitId  IN @AccessPlants ORDER BY QrCode;";

            using var multi = await _dbConnection.GetConnection().QueryMultipleAsync(sql, new { AccessPlants = _iSessionHelper.GetAccessPlantsArray() });
            var mcOwned = await multi.ReadAsync<McOwned>();
            var mcRented = await multi.ReadAsync<McRented>();

            return new MachinesVM
            {
                McOwned = mcOwned,
                McRented = mcRented
            };
        }

        public async Task LoadOwnedMachineListsAsync(McCreateVM mcCreateVM)
        {
            mcCreateVM.MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands");
            mcCreateVM.MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes");
            mcCreateVM.MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels");
            mcCreateVM.GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups");
            mcCreateVM.LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations");
            mcCreateVM.OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters");
            mcCreateVM.OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits");
        }

        public async Task LoadRentedMachineListsAsync(McCreatedRnVM mcCreatedRnVM)
        {
            mcCreatedRnVM.MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineBrands");
            mcCreatedRnVM.MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineTypes");
            mcCreatedRnVM.MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MdMachineModels");
            mcCreatedRnVM.GroupList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyGroups");
            mcCreatedRnVM.LocationsList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyLocations");
            mcCreatedRnVM.OwnedClusterList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyClusters");
            mcCreatedRnVM.OwnedUnitList = await _userControls.LoadDropDownsAsync("SMIM_MdCompanyUnits");
        }

        public async Task<bool> CheckQrAlreadyAvailable(string qrCode)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM SMIM_TrMachineInventory
            WHERE (QrCode = @QrCode) AND IsDelete = 0 ";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { QrCode = qrCode });
            return result.HasValue;
        }

        public async Task<bool> CheckSnAlreadyAvailable(string serialNo)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM SMIM_TrMachineInventory
            WHERE (SerialNo = @SerialNo) AND IsDelete = 0 ";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { SerialNo = serialNo });
            return result.HasValue;
        }

        public async Task<bool> InsertMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            const string query = @"
            INSERT INTO SMIM_TrMachineInventory 
            (QrCode, SerialNo, FarCode, IsOwned, DatePurchased, ServiceSeq,
             MachineBrandId, MachineTypeId, CompanyGroupId, LocationId, OwnedClusterId, OwnedUnitId,
             CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK, DateCreate , IsDelete)
            VALUES 
            (@QrCode, @SerialNo, @FarCode, 1 , @DatePurchased,  @ServiceSeq,
             @MachineBrandId, @MachineTypeId, @CompanyGroupId, @LocationId, @OwnedClusterId, @OwnedUnitId,
             @OwnedUnitId, 1 , @MachineModelId, @Cost, @ImageFR, @ImageBK, @NowDT, 0 )";

            try
            {
                byte[]? imageFrontBytes = null;
                byte[]? imageBackBytes = null;

                if (imageFront != null && imageFront.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFront.CopyToAsync(memoryStream);
                        imageFrontBytes = memoryStream.ToArray();
                    }
                }

                if (imageBack != null && imageBack.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageBack.CopyToAsync(memoryStream);
                        imageBackBytes = memoryStream.ToArray();
                    }
                }

                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    QrCode = mcInventory.QrCode?.ToUpper(),
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    FarCode = mcInventory.FarCode?.ToUpper(),
                    mcInventory.DatePurchased,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.CompanyGroupId,
                    mcInventory.LocationId,
                    mcInventory.OwnedClusterId,
                    mcInventory.OwnedUnitId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                });

                if (insertedId.HasValue)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "MACHINE RECORD CREATED"

                    };

                    _iSMIMLogdb.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<McInventory?> GetMcInventoryByIdAsync(int? id)
        {
            const string query = @"
           SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, ServiceSeq,
           MachineBrandId, MachineTypeId, CompanyGroupId, LocationId, OwnedClusterId, OwnedUnitId,
           CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK FROM SMIM_TrMachineInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<McInventory>(query, new { Id = id });
        }

        public async Task<int> UpdateOwnedMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            // Check if the QR code is already assigned to another machine (excluding the current machine)
            var existingQr = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<McInventory>(
                "SELECT Id FROM SMIM_TrMachineInventory WHERE QrCode = @QrCode AND Id != @Id",
                new { QrCode = mcInventory.QrCode?.ToUpper(), mcInventory.Id });

            if (existingQr != null)
            {
                return 2;
            }

            // Check if the QR code is already assigned to another machine (excluding the current machine)
            var existingSR = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<McInventory>(
                "SELECT Id FROM SMIM_TrMachineInventory WHERE SerialNo = @SerialNo AND Id != @Id",
                new { SerialNo = mcInventory.SerialNo?.ToUpper(), mcInventory.Id });

            if (existingSR != null)
            {
                return 3;
            }

            // Fetch current machine details from database
            var beforeUpdate = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync(
                @"SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, MachineTypeId, MachineType, CompanyGroupId, 
                CompanyGroup, LocationId, Location, OwnedClusterId, OwnedCluster, OwnedUnitId, OwnedUnit, CurrentUnitId, CurrentUnit, MachineModelId, MachineModel, SupplierId, Supplier, 
                CostMethodId, CostMethod, Cost FROM  VwHelpEditLog WHERE  (Id = @Id)",
                new { mcInventory.Id });

            if (beforeUpdate == null)
            {
                return 0; // Machine not found
            }

            var updateFields = new List<string>
            {
                "QrCode = @QrCode",
                "SerialNo = @SerialNo",
                "FarCode = @FarCode",
                "DatePurchased = @DatePurchased",
                "ServiceSeq = @ServiceSeq",
                "MachineBrandId = @MachineBrandId",
                "MachineTypeId = @MachineTypeId",
                "CompanyGroupId = @CompanyGroupId",
                "LocationId = @LocationId",
                "OwnedClusterId = @OwnedClusterId",
                "OwnedUnitId = @OwnedUnitId",
                "CurrentUnitId = @OwnedUnitId",
                "MachineModelId = @MachineModelId",
                "Cost = @Cost",
                "DateUpdate = @NowDT"
            };

            byte[]? imageFrontBytes = null;
            byte[]? imageBackBytes = null;

            if (mcInventory.RemoveImageFront)
            {
                updateFields.Add("ImageFR = NULL");
            }
            else if (imageFront != null && imageFront.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFront.CopyToAsync(memoryStream);
                    imageFrontBytes = memoryStream.ToArray();
                }
                updateFields.Add("ImageFR = @ImageFR");
            }

            if (mcInventory.RemoveImageBack)
            {
                updateFields.Add("ImageBK = NULL");
            }
            else if (imageBack != null && imageBack.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageBack.CopyToAsync(memoryStream);
                    imageBackBytes = memoryStream.ToArray();
                }
                updateFields.Add("ImageBK = @ImageBK");
            }

            var query = $@"UPDATE SMIM_TrMachineInventory SET {string.Join(", ", updateFields)} WHERE Id = @Id";

            try
            {
                var rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    mcInventory.Id,
                    QrCode = mcInventory.QrCode?.ToUpper(),
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    FarCode = mcInventory.FarCode?.ToUpper(),
                    mcInventory.DatePurchased,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.CompanyGroupId,
                    mcInventory.LocationId,
                    mcInventory.OwnedClusterId,
                    mcInventory.OwnedUnitId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                });

                if (rowsAffected > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = mcInventory.Id,
                        TrLog = "MACHINE UPDATED"

                    };

                    var aftertUpdate = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync(
                    @"SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, MachineTypeId, MachineType, CompanyGroupId, 
								CompanyGroup, LocationId, Location, OwnedClusterId, OwnedCluster, OwnedUnitId, OwnedUnit, CurrentUnitId, CurrentUnit, MachineModelId, MachineModel, SupplierId, Supplier, 
								CostMethodId, CostMethod, Cost FROM  VwHelpEditLog WHERE  (Id = @Id)",
                    new { mcInventory.Id });

                    var logChanges = new StringBuilder();

                    LogFieldChanges(logChanges, "QrCode", beforeUpdate.QrCode.ToUpper(), aftertUpdate?.QrCode.ToUpper(), beforeUpdate.QrCode, aftertUpdate?.QrCode);
                    LogFieldChanges(logChanges, "SerialNo", beforeUpdate.SerialNo.ToUpper(), aftertUpdate?.SerialNo.ToUpper(), beforeUpdate.SerialNo, aftertUpdate?.SerialNo);
                    LogFieldChanges(logChanges, "FarCode", beforeUpdate.FarCode.ToUpper(), aftertUpdate?.FarCode.ToUpper(), beforeUpdate.FarCode, aftertUpdate?.FarCode);
                    LogFieldChanges(logChanges, "DatePurchased", beforeUpdate.DatePurchased?.ToString(), aftertUpdate?.DatePurchased?.ToString(), beforeUpdate.DatePurchased?.ToString(), aftertUpdate?.DatePurchased?.ToString());
                    //LogFieldChanges(logChanges, "DateBorrow", beforeUpdate.DateBorrow?.ToString(), aftertUpdate?.DateBorrow?.ToString(), beforeUpdate.DateBorrow?.ToString(), aftertUpdate?.DateBorrow?.ToString());
                    //LogFieldChanges(logChanges, "DateDue", beforeUpdate.DateDue?.ToString(), aftertUpdate?.DateDue?.ToString(), beforeUpdate.DateDue?.ToString(), aftertUpdate?.DateDue?.ToString());
                    LogFieldChanges(logChanges, "ServiceSeq", beforeUpdate.ServiceSeq.ToString(), aftertUpdate?.ServiceSeq.ToString(), beforeUpdate.ServiceSeq.ToString(), aftertUpdate?.ServiceSeq.ToString());
                    LogFieldChanges(logChanges, "MachineBrand", beforeUpdate.MachineBrandId.ToString(), aftertUpdate?.MachineBrandId.ToString(), beforeUpdate.MachineBrand, aftertUpdate?.MachineBrand);
                    LogFieldChanges(logChanges, "MachineType", beforeUpdate.MachineTypeId.ToString(), aftertUpdate?.MachineTypeId.ToString(), beforeUpdate.MachineType, aftertUpdate?.MachineType);
                    LogFieldChanges(logChanges, "CompanyGroup", beforeUpdate.CompanyGroupId.ToString(), aftertUpdate?.CompanyGroupId.ToString(), beforeUpdate.CompanyGroup, aftertUpdate?.CompanyGroup);
                    LogFieldChanges(logChanges, "Location", beforeUpdate.LocationId.ToString(), aftertUpdate?.LocationId.ToString(), beforeUpdate.Location, aftertUpdate?.Location);
                    LogFieldChanges(logChanges, "OwnedCluster", beforeUpdate.OwnedClusterId.ToString(), aftertUpdate?.OwnedClusterId.ToString(), beforeUpdate.OwnedCluster, aftertUpdate?.OwnedCluster);
                    LogFieldChanges(logChanges, "OwnedUnit", beforeUpdate.OwnedUnitId.ToString(), aftertUpdate?.OwnedUnitId.ToString(), beforeUpdate.OwnedUnit, aftertUpdate?.OwnedUnit);
                    LogFieldChanges(logChanges, "CurrentUnit", beforeUpdate.CurrentUnitId.ToString(), aftertUpdate?.CurrentUnitId.ToString(), beforeUpdate.CurrentUnit, aftertUpdate?.CurrentUnit);
                    LogFieldChanges(logChanges, "MachineModel", beforeUpdate.MachineModelId.ToString(), aftertUpdate?.MachineModelId.ToString(), beforeUpdate.MachineModel, aftertUpdate?.MachineModel);
                    //LogFieldChanges(logChanges, "Supplier", beforeUpdate.SupplierId, aftertUpdate?.SupplierId, beforeUpdate.Supplier, aftertUpdate?.Supplier);
                    //LogFieldChanges(logChanges, "CostMethod", beforeUpdate.CostMethodId, aftertUpdate?.CostMethodId, beforeUpdate.CostMethod, aftertUpdate?.CostMethod);
                    LogFieldChanges(logChanges, "Cost", beforeUpdate.Cost.ToString(), aftertUpdate?.Cost.ToString(), beforeUpdate.Cost.ToString(), aftertUpdate?.Cost.ToString());


                    if (logChanges.Length > 0)
                    {
                        logdb.TrLog += "CHANGES: " + logChanges.ToString();
                        _iSMIMLogdb.InsertLog(_dbConnection, logdb);
                    }

                    return 1;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void LogFieldChanges(StringBuilder logChanges, string fieldName, string checkCurrentValue, string checknewValue, string currentValue, string newValue)
        {
            if (checkCurrentValue != checknewValue)
            {
                logChanges.AppendLine($"{fieldName}: {currentValue} -> {newValue}");
            }
        }

        public async Task<bool> InsertRentMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            const string query = @"
        INSERT INTO SMIM_TrMachineInventory 
        (QrCode, SerialNo, IsOwned, DateBorrow, DateDue,  ServiceSeq,
         MachineBrandId, MachineTypeId, CompanyGroupId, LocationId, OwnedClusterId,
         OwnedUnitId, CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK, DateCreate , IsDelete, SupplierId, CostMethodId, Comments )
        VALUES 
        (@QrCode, @SerialNo, 0 , @DateBorrow, @DateDue, @ServiceSeq,
         @MachineBrandId, @MachineTypeId, @CompanyGroupId, @LocationId, @OwnedClusterId,
         @OwnedUnitId, @OwnedUnitId, 1 , @MachineModelId, @Cost, @ImageFR, @ImageBK, @NowDT, 0, @SupplierId, @CostMethodId, @Comments )";

            try
            {
                byte[]? imageFrontBytes = null;
                byte[]? imageBackBytes = null;

                if (imageFront != null && imageFront.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFront.CopyToAsync(memoryStream);
                        imageFrontBytes = memoryStream.ToArray();
                    }
                }

                if (imageBack != null && imageBack.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageBack.CopyToAsync(memoryStream);
                        imageBackBytes = memoryStream.ToArray();
                    }
                }

                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    QrCode = mcInventory.QrCode?.ToUpper(),
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    mcInventory.DateBorrow,
                    mcInventory.DateDue,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.CompanyGroupId,
                    mcInventory.LocationId,
                    mcInventory.OwnedClusterId,
                    mcInventory.OwnedUnitId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    mcInventory.SupplierId,
                    mcInventory.CostMethodId,
                    mcInventory.Comments,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                });

                if (insertedId.HasValue)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "MACHINE RECORD CREATED"
                    };

                    _iSMIMLogdb.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<McInventory?> GetRentMcInventoryByIdAsync(int? id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, DateBorrow, DateDue, ServiceSeq,
            MachineBrandId, MachineTypeId, CompanyGroupId, LocationId, OwnedClusterId, OwnedUnitId,
            CurrentUnitId, CurrentStatusId, MachineModelId, SupplierId, CostMethodId, Cost, ImageFR, ImageBK, Comments
            FROM SMIM_TrMachineInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<McInventory>(query, new { Id = id });
        }

        public async Task<int> UpdateRentMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            // Check if the QR code is already assigned to another machine (excluding the current machine)
            var existingQr = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<McInventory>(
                "SELECT Id FROM SMIM_TrMachineInventory WHERE QrCode = @QrCode AND Id != @Id",
                new { QrCode = mcInventory.QrCode?.ToUpper(), mcInventory.Id });

            if (existingQr != null)
            {
                return 2;
            }

            // Check if the QR code is already assigned to another machine (excluding the current machine)
            var existingSR = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<McInventory>(
                "SELECT Id FROM SMIM_TrMachineInventory WHERE SerialNo = @SerialNo AND Id != @Id",
                new { SerialNo = mcInventory.SerialNo?.ToUpper(), mcInventory.Id });

            if (existingSR != null)
            {
                return 3;
            }

            var beforeUpdate = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync(
            @"SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, MachineTypeId, MachineType, CompanyGroupId, 
						CompanyGroup, LocationId, Location, OwnedClusterId, OwnedCluster, OwnedUnitId, OwnedUnit, CurrentUnitId, CurrentUnit, MachineModelId, MachineModel, SupplierId, Supplier, 
						CostMethodId, CostMethod, Cost FROM  VwHelpEditLog WHERE  (Id = @Id)",
            new { mcInventory.Id });

            if (beforeUpdate == null)
            {
                return 0; // Machine not found
            }


            var updateFields = new List<string>
            {
                "QrCode = @QrCode",
                "SerialNo = @SerialNo",
                "ServiceSeq = @ServiceSeq",
                "MachineModelId = @MachineModelId",
                "MachineBrandId = @MachineBrandId",
                "MachineTypeId = @MachineTypeId",
                "CompanyGroupId = @CompanyGroupId",
                "OwnedClusterId = @OwnedClusterId",
                "OwnedUnitId = @OwnedUnitId",
                "CurrentUnitId = @OwnedUnitId",
                "LocationId = @LocationId",
                "DateBorrow = @DateBorrow",
                "DateDue = @DateDue",
                "Comments = @Comments",
                "SupplierId = @SupplierId",
                "CostMethodId = @CostMethodId",
                "Cost = @Cost",
                "DateUpdate = @NowDT"
            };

            byte[]? imageFrontBytes = null;
            byte[]? imageBackBytes = null;

            if (mcInventory.RemoveImageFront)
            {
                updateFields.Add("ImageFR = NULL");
            }
            else if (imageFront != null && imageFront.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFront.CopyToAsync(memoryStream);
                    imageFrontBytes = memoryStream.ToArray();
                }
                updateFields.Add("ImageFR = @ImageFR");
            }

            if (mcInventory.RemoveImageBack)
            {
                updateFields.Add("ImageBK = NULL");
            }
            else if (imageBack != null && imageBack.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageBack.CopyToAsync(memoryStream);
                    imageBackBytes = memoryStream.ToArray();
                }
                updateFields.Add("ImageBK = @ImageBK");
            }

            var query = $@"UPDATE TrMachineInventory SET {string.Join(", ", updateFields)} WHERE Id = @Id";

            try
            {
                var rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    mcInventory.Id,
                    QrCode = mcInventory.QrCode?.ToUpper(),
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    mcInventory.ServiceSeq,
                    mcInventory.MachineModelId,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.CompanyGroupId,
                    mcInventory.OwnedClusterId,
                    mcInventory.LocationId,
                    mcInventory.OwnedUnitId,
                    mcInventory.DateBorrow,
                    mcInventory.DateDue,
                    Comments = mcInventory.Comments?.ToUpper(),
                    mcInventory.SupplierId,
                    mcInventory.CostMethodId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                });

                if (rowsAffected > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = mcInventory.Id,
                        TrLog = "MACHINE UPDATED"
                    };

                    var aftertUpdate = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync(
                    @"SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, MachineTypeId, MachineType, CompanyGroupId, 
													CompanyGroup, LocationId, Location, OwnedClusterId, OwnedCluster, OwnedUnitId, OwnedUnit, CurrentUnitId, CurrentUnit, MachineModelId, MachineModel, SupplierId, Supplier, 
													CostMethodId, CostMethod, Cost FROM  VwHelpEditLog WHERE  (Id = @Id)",
                    new { mcInventory.Id });


                    var logChanges = new StringBuilder();

                    LogFieldChanges(logChanges, "QrCode", beforeUpdate.QrCode.ToUpper(), aftertUpdate?.QrCode.ToUpper(), beforeUpdate.QrCode, aftertUpdate?.QrCode);
                    LogFieldChanges(logChanges, "SerialNo", beforeUpdate.SerialNo.ToUpper(), aftertUpdate?.SerialNo.ToUpper(), beforeUpdate.SerialNo, aftertUpdate?.SerialNo);
                    LogFieldChanges(logChanges, "FarCode", beforeUpdate.FarCode.ToUpper(), aftertUpdate?.FarCode.ToUpper(), beforeUpdate.FarCode, aftertUpdate?.FarCode);
                    //LogFieldChanges(logChanges, "DatePurchased", beforeUpdate.DatePurchased?.ToString(), aftertUpdate?.DatePurchased?.ToString(), beforeUpdate.DatePurchased?.ToString(), aftertUpdate?.DatePurchased?.ToString());
                    LogFieldChanges(logChanges, "DateBorrow", beforeUpdate.DateBorrow?.ToString(), aftertUpdate?.DateBorrow?.ToString(), beforeUpdate.DateBorrow?.ToString(), aftertUpdate?.DateBorrow?.ToString());
                    LogFieldChanges(logChanges, "DateDue", beforeUpdate.DateDue?.ToString(), aftertUpdate?.DateDue?.ToString(), beforeUpdate.DateDue?.ToString(), aftertUpdate?.DateDue?.ToString());
                    LogFieldChanges(logChanges, "ServiceSeq", beforeUpdate.ServiceSeq.ToString(), aftertUpdate?.ServiceSeq.ToString(), beforeUpdate.ServiceSeq.ToString(), aftertUpdate?.ServiceSeq.ToString());
                    LogFieldChanges(logChanges, "MachineBrand", beforeUpdate.MachineBrandId.ToString(), aftertUpdate?.MachineBrandId.ToString(), beforeUpdate.MachineBrand, aftertUpdate?.MachineBrand);
                    LogFieldChanges(logChanges, "MachineType", beforeUpdate.MachineTypeId.ToString(), aftertUpdate?.MachineTypeId.ToString(), beforeUpdate.MachineType, aftertUpdate?.MachineType);
                    LogFieldChanges(logChanges, "CompanyGroup", beforeUpdate.CompanyGroupId.ToString(), aftertUpdate?.CompanyGroupId.ToString(), beforeUpdate.CompanyGroup, aftertUpdate?.CompanyGroup);
                    LogFieldChanges(logChanges, "Location", beforeUpdate.LocationId.ToString(), aftertUpdate?.LocationId.ToString(), beforeUpdate.Location, aftertUpdate?.Location);
                    LogFieldChanges(logChanges, "OwnedCluster", beforeUpdate.OwnedClusterId.ToString(), aftertUpdate?.OwnedClusterId.ToString(), beforeUpdate.OwnedCluster, aftertUpdate?.OwnedCluster);
                    LogFieldChanges(logChanges, "OwnedUnit", beforeUpdate.OwnedUnitId.ToString(), aftertUpdate?.OwnedUnitId.ToString(), beforeUpdate.OwnedUnit, aftertUpdate?.OwnedUnit);
                    LogFieldChanges(logChanges, "CurrentUnit", beforeUpdate.CurrentUnitId.ToString(), aftertUpdate?.CurrentUnitId.ToString(), beforeUpdate.CurrentUnit, aftertUpdate?.CurrentUnit);
                    LogFieldChanges(logChanges, "MachineModel", beforeUpdate.MachineModelId.ToString(), aftertUpdate?.MachineModelId.ToString(), beforeUpdate.MachineModel, aftertUpdate?.MachineModel);
                    LogFieldChanges(logChanges, "Supplier", beforeUpdate.SupplierId.ToString(), aftertUpdate?.SupplierId.ToString(), beforeUpdate.Supplier, aftertUpdate?.Supplier);
                    LogFieldChanges(logChanges, "CostMethod", beforeUpdate.CostMethodId.ToString(), aftertUpdate?.CostMethodId.ToString(), beforeUpdate.CostMethod, aftertUpdate?.CostMethod);
                    LogFieldChanges(logChanges, "Cost", beforeUpdate.Cost.ToString(), aftertUpdate?.Cost.ToString(), beforeUpdate.Cost.ToString(), aftertUpdate?.Cost.ToString());


                    if (logChanges.Length > 0)
                    {
                        logdb.TrLog += "CHANGES: " + logChanges.ToString();
                        _iSMIMLogdb.InsertLog(_dbConnection, logdb);
                    }
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<MachineOwnedVM?> GetOwnedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, ServiceSeq, MachineBrand, MachineType, CompanyGroup, Location, OwnedCluster, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, 
            LastScanDateTime FROM VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineOwnedVM>(query, new { Id = id });
        }

        public async Task<MachineRentedVM?> GetRentedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DateBorrow, DateDue, ServiceSeq, MachineBrand, MachineType, CompanyGroup, Location, OwnedCluster, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, Supplier, CostMethod, Comments,
            LastScanDateTime FROM VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineRentedVM>(query, new { Id = id });
        }

       
    }
}
