using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.Text;
using System.Transactions;
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
                    BrandsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands"),
                    TypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes"),
                    ModelsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels"),
                    LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines"),
                    OwnedLocList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs"),
                    McInventory = new McInventory()
                };
            }
            else
            {
                oMcCreateVM = new McCreateVM
                {
                    BrandsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands"),
                    TypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes"),
                    ModelsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels"),
                    LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines"),
                    OwnedLocList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs"),
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
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels"),
                    LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs"),
                    SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoRentSuppliers"),
                    CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoCostDuration"),
                    McInventory = new McInventory()
                };
            }
            else
            {
                oMcCreatedRnVM = new McCreatedRnVM
                {
                    MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands"),
                    MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes"),
                    MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels"),
                    LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines"),
                    OwnedUnitList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs"),
                    SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoRentSuppliers"),
                    CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoCostDuration"),

                    McInventory = await GetRentMcInventoryByIdAsync(id)
                };
            }
            return oMcCreatedRnVM;
        }

        public async Task<MachinesVM> GetList()
        {
            var sql = @"
            SELECT Id, QrCode, SerialNo, MachineBrand, MachineType FROM SMIM_VwMcInventory WHERE IsOwned = 1 AND [CurrentStatus] NOT IN (9) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;
            SELECT Id, QrCode, SerialNo, DateBorrow, MachineType FROM SMIM_VwMcInventory WHERE IsOwned = 0 AND [CurrentStatus] NOT IN (10) AND CurrentUnitId  IN @AccessPlants ORDER BY QrCode;";

            using var multi = await _dbConnection.GetConnection().QueryMultipleAsync(sql, new { AccessPlants = _iSessionHelper.GetLocationList() });
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
            mcCreateVM.BrandsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands");
            mcCreateVM.TypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes");
            mcCreateVM.ModelsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels");
            mcCreateVM.LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines");
            mcCreateVM.OwnedLocList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs");
        }

        public async Task LoadRentedMachineListsAsync(McCreatedRnVM mcCreatedRnVM)
        {
            mcCreatedRnVM.MachineBrandList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoBrands");
            mcCreatedRnVM.MachineTypesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoTypes");
            mcCreatedRnVM.MachineModelList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoModels");
            mcCreatedRnVM.LinesList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoSewingLines");
            mcCreatedRnVM.OwnedUnitList = await _userControls.LoadDropDownsAsync("COMN_VwTwoCompLocs");
        }

        public async Task<bool> CheckQrAlreadyAvailable(string qrCode)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM SMIM_TrInventory
            WHERE (QrCode = @QrCode) AND IsDelete = 0 ";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { QrCode = qrCode });
            return result.HasValue;
        }

        public async Task<bool> CheckSnAlreadyAvailable(string serialNo)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM SMIM_TrInventory
            WHERE (SerialNo = @SerialNo) AND IsDelete = 0 ";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { SerialNo = serialNo });
            return result.HasValue;
        }

        public async Task<bool> InsertMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            const string query = @"
            INSERT INTO SMIM_TrInventory 
            (QrCode, SerialNo, FarCode, IsOwned, DatePurchased, ServiceSeq,
             MachineBrandId, MachineTypeId,LocationId, OwnedUnitId,
             CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK, DateCreate , IsDelete) OUTPUT INSERTED.Id
            VALUES 
            (@QrCode, @SerialNo, @FarCode, 1 , @DatePurchased,  @ServiceSeq,
             @MachineBrandId, @MachineTypeId, @LocationId, @OwnedUnitId,
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

                string referenceNumber = await _userControls.GenerateSeqRefAsync(connection, transaction, "[SMIM_XysGenerateNumber]", "TSM");

                var insertedId = await connection.QuerySingleOrDefaultAsync<int?>(query, new
                {
                    QrCode = referenceNumber,
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    FarCode = mcInventory.FarCode?.ToUpper(),
                    mcInventory.DatePurchased,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.LocationId,
                    mcInventory.OwnedUnitId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                }, transaction);

                Logdb logdb = new()
                {
                    TrObjectId = insertedId.Value,
                    TrLog = "MACHINE RECORD CREATED"
                };

                _iSMIMLogdb.InsertLog(connection, logdb, transaction);

                transaction.Commit();
                return insertedId > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<McInventory?> GetMcInventoryByIdAsync(int? id)
        {
            const string query = @"
           SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, ServiceSeq,
           MachineBrandId, MachineTypeId, LocationId, OwnedUnitId,
           CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK FROM SMIM_TrInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<McInventory>(query, new { Id = id });
        }

        public async Task<int> UpdateOwnedMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Check duplicate QrCode
                var existingQr = await connection.QueryFirstOrDefaultAsync<int?>(
                    @"SELECT Id FROM SMIM_TrInventory WHERE QrCode = @QrCode AND Id != @Id",
                    new { QrCode = mcInventory.QrCode?.ToUpper(), mcInventory.Id }, transaction);

                if (existingQr != null)
                    return 2;

                // Check duplicate SerialNo
                var existingSR = await connection.QueryFirstOrDefaultAsync<int?>(
                    @"SELECT Id FROM SMIM_TrInventory WHERE SerialNo = @SerialNo AND Id != @Id",
                    new { SerialNo = mcInventory.SerialNo?.ToUpper(), mcInventory.Id }, transaction);

                if (existingSR != null)
                    return 3;

                // Get current machine state
                var beforeUpdate = await connection.QueryFirstOrDefaultAsync(
                    @"SELECT Id, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, 
                     MachineTypeId, MachineType, MachineModelId, MachineModel, SupplierId, Supplier, CostMethodId, CostMethod, Cost 
              FROM SMIM_VwHelpEditLog WHERE Id = @Id",
                    new { mcInventory.Id }, transaction);

                if (beforeUpdate == null)
                    return 0;

                // Prepare update fields
                var updateFields = new List<string>
                {
                    "SerialNo = @SerialNo",
                    "FarCode = @FarCode",
                    "DatePurchased = @DatePurchased",
                    "ServiceSeq = @ServiceSeq",
                    "MachineBrandId = @MachineBrandId",
                    "MachineTypeId = @MachineTypeId",
                    "MachineModelId = @MachineModelId",
                    "Cost = @Cost",
                    "DateUpdate = @NowDT"
                };

                byte[]? imageFrontBytes = null;
                byte[]? imageBackBytes = null;

                if (mcInventory.RemoveImageFront)
                    updateFields.Add("ImageFR = NULL");
                else if (imageFront != null && imageFront.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await imageFront.CopyToAsync(ms);
                    imageFrontBytes = ms.ToArray();
                    updateFields.Add("ImageFR = @ImageFR");
                }

                if (mcInventory.RemoveImageBack)
                    updateFields.Add("ImageBK = NULL");
                else if (imageBack != null && imageBack.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await imageBack.CopyToAsync(ms);
                    imageBackBytes = ms.ToArray();
                    updateFields.Add("ImageBK = @ImageBK");
                }

                var updateQuery = $@"UPDATE SMIM_TrInventory SET {string.Join(", ", updateFields)} WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(updateQuery, new
                {
                    mcInventory.Id,                    
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    FarCode = mcInventory.FarCode?.ToUpper(),
                    mcInventory.DatePurchased,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                }, transaction);

                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                var afterUpdate = await connection.QueryFirstOrDefaultAsync(
                    @"SELECT Id, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, 
                     MachineTypeId, MachineType, MachineModelId, MachineModel, SupplierId, Supplier, CostMethodId, CostMethod, Cost 
              FROM SMIM_VwHelpEditLog WHERE Id = @Id",
                    new { mcInventory.Id }, transaction);

                var logChanges = new StringBuilder();
                LogFieldChanges(logChanges, "SerialNo", beforeUpdate.SerialNo.ToUpper(), afterUpdate?.SerialNo.ToUpper(), beforeUpdate.SerialNo, afterUpdate?.SerialNo);
                LogFieldChanges(logChanges, "FarCode", beforeUpdate.FarCode.ToUpper(), afterUpdate?.FarCode.ToUpper(), beforeUpdate.FarCode, afterUpdate?.FarCode);
                LogFieldChanges(logChanges, "DatePurchased", beforeUpdate.DatePurchased?.ToString(), afterUpdate?.DatePurchased?.ToString(), beforeUpdate.DatePurchased?.ToString(), afterUpdate?.DatePurchased?.ToString());
                LogFieldChanges(logChanges, "ServiceSeq", beforeUpdate.ServiceSeq?.ToString(), afterUpdate?.ServiceSeq?.ToString(), beforeUpdate.ServiceSeq?.ToString(), afterUpdate?.ServiceSeq?.ToString());
                LogFieldChanges(logChanges, "MachineBrand", beforeUpdate.MachineBrandId?.ToString(), afterUpdate?.MachineBrandId?.ToString(), beforeUpdate.MachineBrand, afterUpdate?.MachineBrand);
                LogFieldChanges(logChanges, "MachineType", beforeUpdate.MachineTypeId?.ToString(), afterUpdate?.MachineTypeId?.ToString(), beforeUpdate.MachineType, afterUpdate?.MachineType);
                LogFieldChanges(logChanges, "MachineModel", beforeUpdate.MachineModelId?.ToString(), afterUpdate?.MachineModelId?.ToString(), beforeUpdate.MachineModel, afterUpdate?.MachineModel);
                LogFieldChanges(logChanges, "Cost", beforeUpdate.Cost?.ToString(), afterUpdate?.Cost?.ToString(), beforeUpdate.Cost?.ToString(), afterUpdate?.Cost?.ToString());

                if (logChanges.Length > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = mcInventory.Id,
                        TrLog = "MACHINE UPDATED. CHANGES: " + logChanges.ToString()
                    };
                    _iSMIMLogdb.InsertLog(connection, logdb, transaction);
                }

                transaction.Commit();
                return 1;
            }
            catch
            {
                transaction.Rollback();
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
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();


            const string query = @"
        INSERT INTO SMIM_TrInventory 
        (QrCode, SerialNo, IsOwned, DateBorrow, DateDue,  ServiceSeq,
         MachineBrandId, MachineTypeId, LocationId, OwnedUnitId, CurrentUnitId, CurrentStatusId, MachineModelId, Cost, ImageFR, ImageBK, DateCreate , IsDelete, SupplierId, CostMethodId, Comments ) OUTPUT INSERTED.Id
        VALUES 
        (@QrCode, @SerialNo, 0 , @DateBorrow, @DateDue, @ServiceSeq,
         @MachineBrandId, @MachineTypeId, @LocationId, @OwnedUnitId, @OwnedUnitId, 1 , @MachineModelId, @Cost, @ImageFR, @ImageBK, @NowDT, 0, @SupplierId, @CostMethodId, @Comments )";

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

                string referenceNumber = await _userControls.GenerateSeqRefAsync(connection, transaction, "[SMIM_XysGenerateNumber]", "TSM");

                var insertedId = await connection.QuerySingleOrDefaultAsync<int?>(query, new
                {
                    QrCode = referenceNumber,
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    mcInventory.DateBorrow,
                    mcInventory.DateDue,
                    mcInventory.ServiceSeq,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.LocationId,
                    mcInventory.OwnedUnitId,
                    mcInventory.MachineModelId,
                    mcInventory.Cost,
                    mcInventory.SupplierId,
                    mcInventory.CostMethodId,
                    mcInventory.Comments,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                }, transaction);

                Logdb logdb = new()
                {
                    TrObjectId = insertedId.Value,
                    TrLog = "MACHINE RECORD CREATED"
                };

                _iSMIMLogdb.InsertLog(connection, logdb, transaction);


                transaction.Commit();
                return insertedId > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<McInventory?> GetRentMcInventoryByIdAsync(int? id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, DateBorrow, DateDue, ServiceSeq,
            MachineBrandId, MachineTypeId, LocationId, OwnedUnitId,
            CurrentUnitId, CurrentStatusId, MachineModelId, SupplierId, CostMethodId, Cost, ImageFR, ImageBK, Comments
            FROM SMIM_TrInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<McInventory>(query, new { Id = id });
        }

        public async Task<int> UpdateRentMachineAsync(McInventory mcInventory, IFormFile? imageFront, IFormFile? imageBack)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            // Check duplicate QrCode
            var existingQr = await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT Id FROM SMIM_TrInventory WHERE QrCode = @QrCode AND Id != @Id",
                new { QrCode = mcInventory.QrCode?.ToUpper(), mcInventory.Id }, transaction);

            if (existingQr != null)
                return 2;

            // Check duplicate SerialNo
            var existingSR = await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT Id FROM SMIM_TrInventory WHERE SerialNo = @SerialNo AND Id != @Id",
                new { SerialNo = mcInventory.SerialNo?.ToUpper(), mcInventory.Id }, transaction);

            if (existingSR != null)
                return 3;

            // Get current machine state
            var beforeUpdate = await connection.QueryFirstOrDefaultAsync(
                @"SELECT Id, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, 
                     MachineTypeId, MachineType, MachineModelId, MachineModel, SupplierId, Supplier, CostMethodId, CostMethod, Cost 
              FROM SMIM_VwHelpEditLog WHERE Id = @Id",
                new { mcInventory.Id }, transaction);

            if (beforeUpdate == null)
                return 0;


            var updateFields = new List<string>
            {
                "SerialNo = @SerialNo",
                "ServiceSeq = @ServiceSeq",
                "MachineModelId = @MachineModelId",
                "MachineBrandId = @MachineBrandId",
                "MachineTypeId = @MachineTypeId",
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
                updateFields.Add("ImageFR = NULL");
            else if (imageFront != null && imageFront.Length > 0)
            {
                using var ms = new MemoryStream();
                await imageFront.CopyToAsync(ms);
                imageFrontBytes = ms.ToArray();
                updateFields.Add("ImageFR = @ImageFR");
            }

            if (mcInventory.RemoveImageBack)
                updateFields.Add("ImageBK = NULL");
            else if (imageBack != null && imageBack.Length > 0)
            {
                using var ms = new MemoryStream();
                await imageBack.CopyToAsync(ms);
                imageBackBytes = ms.ToArray();
                updateFields.Add("ImageBK = @ImageBK");
            }

            var query = $@"UPDATE SMIM_TrInventory SET {string.Join(", ", updateFields)} WHERE Id = @Id";

            try
            {
                var rowsAffected = await connection.ExecuteAsync(query, new
                {
                    mcInventory.Id,
                    SerialNo = mcInventory.SerialNo?.ToUpper(),
                    mcInventory.ServiceSeq,
                    mcInventory.MachineModelId,
                    mcInventory.MachineBrandId,
                    mcInventory.MachineTypeId,
                    mcInventory.DateBorrow,
                    mcInventory.DateDue,
                    Comments = mcInventory.Comments?.ToUpper(),
                    mcInventory.SupplierId,
                    mcInventory.CostMethodId,
                    mcInventory.Cost,
                    ImageFR = imageFrontBytes,
                    ImageBK = imageBackBytes,
                    NowDT = DateTime.Now
                }, transaction);

                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                var afterUpdate = await connection.QueryFirstOrDefaultAsync(
                  @"SELECT Id, SerialNo, FarCode, DatePurchased, DateBorrow, DateDue, ServiceSeq, MachineBrandId, MachineBrand, MachineTypeId, MachineType, 
					MachineModelId, MachineModel, SupplierId, Supplier, 
					CostMethodId, CostMethod, Cost FROM  SMIM_VwHelpEditLog WHERE  (Id = @Id)",
                  new { mcInventory.Id }, transaction);

                var logChanges = new StringBuilder();
                LogFieldChanges(logChanges, "SerialNo", beforeUpdate.SerialNo.ToUpper(), afterUpdate?.SerialNo.ToUpper(), beforeUpdate.SerialNo, afterUpdate?.SerialNo);
                LogFieldChanges(logChanges, "FarCode", beforeUpdate.FarCode.ToUpper(), afterUpdate?.FarCode.ToUpper(), beforeUpdate.FarCode, afterUpdate?.FarCode);
                LogFieldChanges(logChanges, "DateBorrow", beforeUpdate.DateBorrow?.ToString(), afterUpdate?.DateBorrow?.ToString(), beforeUpdate.DateBorrow?.ToString(), afterUpdate?.DateBorrow?.ToString());
                LogFieldChanges(logChanges, "DateDue", beforeUpdate.DateDue?.ToString(), afterUpdate?.DateDue?.ToString(), beforeUpdate.DateDue?.ToString(), afterUpdate?.DateDue?.ToString());
                LogFieldChanges(logChanges, "ServiceSeq", beforeUpdate.ServiceSeq.ToString(), afterUpdate?.ServiceSeq.ToString(), beforeUpdate.ServiceSeq.ToString(), afterUpdate?.ServiceSeq.ToString());
                LogFieldChanges(logChanges, "MachineBrand", beforeUpdate.MachineBrandId.ToString(), afterUpdate?.MachineBrandId.ToString(), beforeUpdate.MachineBrand, afterUpdate?.MachineBrand);
                LogFieldChanges(logChanges, "MachineType", beforeUpdate.MachineTypeId.ToString(), afterUpdate?.MachineTypeId.ToString(), beforeUpdate.MachineType, afterUpdate?.MachineType);
                LogFieldChanges(logChanges, "MachineModel", beforeUpdate.MachineModelId.ToString(), afterUpdate?.MachineModelId.ToString(), beforeUpdate.MachineModel, afterUpdate?.MachineModel);
                LogFieldChanges(logChanges, "Supplier", beforeUpdate.SupplierId.ToString(), afterUpdate?.SupplierId.ToString(), beforeUpdate.Supplier, afterUpdate?.Supplier);
                LogFieldChanges(logChanges, "CostMethod", beforeUpdate.CostMethodId.ToString(), afterUpdate?.CostMethodId.ToString(), beforeUpdate.CostMethod, afterUpdate?.CostMethod);
                LogFieldChanges(logChanges, "Cost", beforeUpdate.Cost.ToString(), afterUpdate?.Cost.ToString(), beforeUpdate.Cost.ToString(), afterUpdate?.Cost.ToString());

                if (logChanges.Length > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = mcInventory.Id,
                        TrLog = "MACHINE UPDATED. CHANGES: " + logChanges.ToString()
                    };
                    _iSMIMLogdb.InsertLog(connection, logdb, transaction);
                }

                transaction.Commit();
                return 1;
            }
            catch
            {
                transaction.Rollback();
                return 0;
            }
        }

        public async Task<MachineOwnedVM?> GetOwnedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DatePurchased, ServiceSeq, MachineBrand, MachineType, Location, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, 
            LastScanDateTime FROM SMIM_VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineOwnedVM>(query, new { Id = id });
        }

        public async Task<MachineRentedVM?> GetRentedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DateBorrow, DateDue, ServiceSeq, MachineBrand, MachineType, Location, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, Supplier, CostMethod, Comments,
            LastScanDateTime FROM SMIM_VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineRentedVM>(query, new { Id = id });
        }


    }
}
