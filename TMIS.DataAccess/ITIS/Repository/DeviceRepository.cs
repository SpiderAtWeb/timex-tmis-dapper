using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Cms;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList, ISessionHelper sessionHelper,
                            IITISLogdb iITISLogdb): IDeviceRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            string sql = @"select d.DeviceID, d.DeviceName, d.SerialNumber, d.FixedAssetCode, s.PropName as Status,
                            v.Name as Vendor, d.PurchasedDate, d.Remark, t.DeviceType
                            from ITIS_Devices as d 
                            inner join COMN_MasterTwoLocations as l on l.Id=d.Location
                            inner join ITIS_DeviceStatus as s on s.Id=d.DeviceStatusID
                            inner join ITIS_VendorTemp as v on v.ID=d.VendorID
                            inner join ITIS_DeviceTypes as t on t.DeviceTypeID=d.DeviceTypeID";

            return await _dbConnection.GetConnection().QueryAsync<Device>(sql);
        }

        public async Task<CreateDeviceVM> LoadDropDowns()
        {
            var objCreateDeviceVM = new CreateDeviceVM();

            objCreateDeviceVM = new CreateDeviceVM
            {
                LocationList = await _icommonList.LoadLocations(),
                DeviceTypeList = await _icommonList.LoadDeviceTypes(),
                DeviceStatusList = await _icommonList.LoadDeviceStatus(),
                VendorsList = await _icommonList.LoadVendors()
            };

            return objCreateDeviceVM;
        }

        public async Task<CreateDeviceVM> GetAllAttributes(int deviceID)
        {
            var objCreateDeviceVM = new CreateDeviceVM();

            string sql = @"SELECT att.AttributeID, att.Name, att.DataType, ty.AttributeType
                            FROM ITIS_Attributes AS att
                            INNER JOIN ITIS_AttributeType AS ty ON ty.ID = att.DataType
                            WHERE att.DeviceTypeID=@DeviceTypeID";

            var attributes = (await _dbConnection.GetConnection().QueryAsync<AttributeWithOptionsVM>(sql, new {
                DeviceTypeID = deviceID
            })).ToList();

            // Load pick list options
            foreach (var attr in attributes.Where(a => a.DataType == 1))
            {
                var optionSql = "SELECT OptionText FROM ITIS_AttributeListOptions WHERE AttributeID = @AttributeID";
                var options = await _dbConnection.GetConnection().QueryAsync<string>(optionSql, new {
                    AttributeID = attr.AttributeID 
                });
                attr.Options = options.ToList();
            }

            objCreateDeviceVM.Attributes = attributes;  

            return objCreateDeviceVM;   
        }

        public async Task<bool> CheckSerialNumberExist(string serialNumber)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_Devices
            WHERE SerialNumber = @SerialNumber"
            ;

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { SerialNumber = serialNumber });
            return result.HasValue;
        }
        public async Task<bool> AddAsync(CreateDeviceVM obj, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4)
        {


            const string query = @"INSERT INTO ITIS_Devices (
                DeviceTypeID, DeviceName, SerialNumber, FixedAssetCode, Location,
                Image1Data, Image2Data, Image3Data, Image4Data, PurchasedDate,
                AddedBy, DeviceStatusID, Remark, depreciation, VendorID,
                IsRented, IsBrandNew
            ) VALUES (
                @DeviceTypeID, @DeviceName, @SerialNumber, @FixedAssetCode, @Location,
                @Image1Data, @Image2Data, @Image3Data, @Image4Data, @PurchasedDate,
                @AddedBy, @DeviceStatusID, @Remark, @Depreciation, @VendorID,
                @IsRented, @IsBrandNew
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                byte[]? image1Bytes = null;
                byte[]? image2Bytes = null;
                byte[]? image3Bytes = null;
                byte[]? image4Bytes = null;


                if (image1 != null && image1.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image1.CopyToAsync(memoryStream);
                        image1Bytes = memoryStream.ToArray();
                    }
                }
                if (image2 != null && image2.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image2.CopyToAsync(memoryStream);
                        image2Bytes = memoryStream.ToArray();
                    }
                }
                if (image3 != null && image3.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image3.CopyToAsync(memoryStream);
                        image3Bytes = memoryStream.ToArray();
                    }
                }
                if (image4 != null && image4.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image4.CopyToAsync(memoryStream);
                        image4Bytes = memoryStream.ToArray();
                    }
                }
           
                using (var trns = _dbConnection.GetConnection().BeginTransaction())
                {
                    try
                    {
                        var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                        {
                            DeviceTypeID = obj.Device!.DeviceTypeID,
                            DeviceName = obj.Device.DeviceName,
                            SerialNumber = obj.Device.SerialNumber,
                            FixedAssetCode = obj.Device.FixedAssetCode,
                            Location = obj.Device.Location,
                            Image1Data = image1Bytes,
                            Image2Data = image2Bytes,
                            Image3Data = image3Bytes,
                            Image4Data = image4Bytes,
                            PurchasedDate = obj.Device.PurchasedDate,
                            AddedBy = _iSessionHelper.GetShortName().ToUpper(),
                            DeviceStatusID = obj.Device.DeviceStatusID,
                            Remark = obj.Device.Remark,
                            Depreciation = obj.Device.Depreciation,
                            VendorID = obj.Device.VendorID,
                            IsRented = obj.Device.IsRented,
                            IsBrandNew = obj.Device.IsBrandNew
                        }, trns);

                        if (insertedId.HasValue)
                        {
                            if (obj.Attributes != null && obj.Attributes.Any())
                            {
                                const string attributeQuery = @"INSERT INTO ITIS_DeviceAttributeValues
                                (DeviceID,AttributeID,ValueText)
                                VALUES (@DeviceID,@AttributeID,@ValueText)";

                                foreach (var option in obj.Attributes)
                                {
                                    await _dbConnection.GetConnection().ExecuteAsync(attributeQuery, new
                                    {
                                        DeviceID = insertedId.Value,
                                        AttributeID = option.AttributeID,
                                        ValueText = option.Value
                                    }, trns);
                                }
                            }

                            // Commit the transaction
                            trns.Commit();

                            Models.ITIS.Logdb logdb = new()
                            {
                                TrObjectId = insertedId.Value,
                                TrLog = "DEVICE CREATED"

                            };

                            _iITISLogdb.InsertLog(_dbConnection, logdb);
                        }

                        return insertedId > 0;
                    }
                    catch
                    {
                        // Rollback the transaction if any command fails
                        trns.Rollback();
                        throw;
                    }
                                  
                }
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}
