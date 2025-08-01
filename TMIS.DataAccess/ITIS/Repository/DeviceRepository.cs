using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList, ISessionHelper sessionHelper,
                            IITISLogdb iITISLogdb) : IDeviceRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            string sql = @"select d.DeviceID,l.PropName as Location, t.DeviceType, d.DeviceName, d.SerialNumber, dd.DepartmentName as Department,
                        s.PropName as Status, ISNULL(da.EmpName, '-') AS EmpName
                        from ITIS_Devices as d 
                        left join COMN_MasterTwoLocations as l on l.Id=d.Location
                        left join COMN_MasterDepartments as dd on dd.DepartmentID=d.Department
                        left join ITIS_DeviceStatus as s on s.Id=d.DeviceStatusID
                        left join ITIS_VendorTemp as v on v.ID=d.VendorID
                        left join ITIS_DeviceTypes as t on t.DeviceTypeID=d.DeviceTypeID
						LEFT JOIN ITIS_DeviceAssignments AS da 
                        ON da.DeviceID = d.DeviceID AND da.AssignStatusID = 3";

            return await _dbConnection.GetConnection().QueryAsync<Device>(sql);
        }

        public async Task<CreateDeviceVM> LoadDropDowns(int? deviceID)
        {
            var objCreateDeviceVM = new CreateDeviceVM();

            objCreateDeviceVM = new CreateDeviceVM
            {
                LocationList = await _icommonList.LoadLocations(),
                DeviceTypeList = await _icommonList.LoadDeviceTypes(),
                DeviceStatusList = await _icommonList.LoadDeviceStatus(),
                VendorsList = await _icommonList.LoadVendors(),
                DepartmentList = await _icommonList.LoadDepartments()
            };

            if (deviceID != null && deviceID.HasValue)
            {
                objCreateDeviceVM.Device = await GetDeviceDetail(deviceID);
            }

            return objCreateDeviceVM;
        }

        public async Task<Device> GetDeviceDetail(int? deviceID)
        {
            string query = @"select * from ITIS_Devices where DeviceID=@DeviceID";

            Device device = new Device();
            var deviceDetail = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<Device>(query, new
            {
                DeviceID = deviceID
            });

            device = deviceDetail!;

            string attributeQuery = @"select att.ID as AttributeType, av.AttributeID, a.Name, av.ValueText from ITIS_DeviceAttributeValues as av 
                                     inner join ITIS_Attributes as a on a.AttributeID=av.AttributeID 
									 inner join ITIS_AttributeType as att on att.ID=a.DataType where av.DeviceID=@DeviceID";

            var attributeValue = await _dbConnection.GetConnection().QueryAsync<AttributeValue>(attributeQuery, new
            {
                DeviceID = deviceID
            });

            device.AttributeValues = attributeValue.ToList();

            return device;

        }

        public async Task<CreateDeviceVM> GetAllAttributes(int deviceID)
        {
            var objCreateDeviceVM = new CreateDeviceVM();

            string sql = @"SELECT att.AttributeID, att.Name, att.DataType, ty.AttributeType
                            FROM ITIS_Attributes AS att
                            INNER JOIN ITIS_AttributeType AS ty ON ty.ID = att.DataType
                            WHERE att.DeviceTypeID=@DeviceTypeID and att.IsDelete=0";

            var attributes = (await _dbConnection.GetConnection().QueryAsync<AttributeWithOptionsVM>(sql, new
            {
                DeviceTypeID = deviceID
            })).ToList();

            // Load pick list options
            foreach (var attr in attributes.Where(a => a.DataType == 1))
            {
                var optionSql = "SELECT OptionText FROM ITIS_AttributeListOptions WHERE AttributeID = @AttributeID";
                var options = await _dbConnection.GetConnection().QueryAsync<string>(optionSql, new
                {
                    AttributeID = attr.AttributeID
                });
                attr.Options = options.ToList();
            }

            objCreateDeviceVM.Attributes = attributes;

            return objCreateDeviceVM;
        }

        public async Task<bool> CheckSerialEdit(string serialNumber, int deviceID)
        {
            const string query = @"SELECT TOP 1 1
            FROM ITIS_Devices
            WHERE SerialNumber = @SerialNumber and DeviceID!=@DeviceID;";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new
            {
                SerialNumber = serialNumber,
                DeviceID = deviceID
            });
            return result.HasValue;

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
                IsRented, IsBrandNew, Department
            ) VALUES (
                @DeviceTypeID, @DeviceName, @SerialNumber, @FixedAssetCode, @Location,
                @Image1Data, @Image2Data, @Image3Data, @Image4Data, @PurchasedDate,
                @AddedBy, @DeviceStatusID, @Remark, @Depreciation, @VendorID,
                @IsRented, @IsBrandNew, @Department
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
                            IsBrandNew = obj.Device.IsBrandNew,
                            Department = obj.Device.Department
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

        public async Task<DeviceDetailVM> LoadDeviceDetail(int deviceID)
        {
            var objDeviceDetailVM = new DeviceDetailVM();

            objDeviceDetailVM = await _icommonList.LoadDeviceDetail(deviceID);

            return objDeviceDetailVM;
        }

        public async Task<DeviceUserDetailVM> LoadUserDetail(int deviceID)
        {
            var objDeviceUserDetailVM = new DeviceUserDetailVM();

            objDeviceUserDetailVM = await _icommonList.LoadUserDetail(deviceID);

            return objDeviceUserDetailVM;
        }

        public async Task<bool> UpdateDevice(CreateDeviceVM obj, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4)
        {
            var updateFields = new List<string>
            {
                "DeviceName = @DeviceName",
                "SerialNumber = @SerialNumber",
                "FixedAssetCode = @FixedAssetCode",
                "Location = @Location",
                "PurchasedDate = @PurchasedDate",
                "DeviceStatusID = @DeviceStatusID",
                "Remark = @Remark",
                "Depreciation = @Depreciation",
                "VendorID = @VendorID",
                "IsRented = @IsRented",
                "UpdatedOn = @UpdatedOn",
                "IsBrandNew = @IsBrandNew"
            };

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
                if (image1Bytes != null && image1Bytes.Length > 0)
                {
                    updateFields.Add("Image1Data = @Image1Bytes");
                }
                if (image2Bytes != null && image2Bytes.Length > 0)
                {
                    updateFields.Add("Image2Data = @Image2Bytes");
                }
                if (image3Bytes != null && image3Bytes.Length > 0)
                {
                    updateFields.Add("Image3Data = @Image3Bytes");
                }
                if (image4Bytes != null && image4Bytes.Length > 0)
                {
                    updateFields.Add("Image4Data = @Image4Bytes");
                }

                var query = $@"UPDATE ITIS_Devices SET {string.Join(", ", updateFields)} where DeviceID=@DeviceID;";

                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    DeviceID = obj.Device!.DeviceID,
                    DeviceName = obj.Device!.DeviceName,
                    SerialNumber = obj.Device.SerialNumber,
                    FixedAssetCode = obj.Device.FixedAssetCode,
                    Location = obj.Device.Location,
                    Image1Bytes = image1Bytes,
                    Image2Bytes = image2Bytes,
                    Image3Bytes = image3Bytes,
                    Image4Bytes = image4Bytes,
                    PurchasedDate = obj.Device.PurchasedDate,
                    UpdatedOn = DateTime.Now,
                    DeviceStatusID = obj.Device.DeviceStatusID,
                    Remark = obj.Device.Remark,
                    Depreciation = obj.Device.Depreciation,
                    VendorID = obj.Device.VendorID,
                    IsRented = obj.Device.IsRented,
                    IsBrandNew = obj.Device.IsBrandNew
                });

                if (rowsAffected > 0)
                {
                    if (obj.Attributes != null && obj.Attributes!.Any())
                    {
                        using (var trns = _dbConnection.GetConnection().BeginTransaction())
                        {
                            try
                            {
                                const string deleteAttributeValue = @"DELETE FROM ITIS_DeviceAttributeValues WHERE DeviceID=@DeviceID; ";

                                await _dbConnection.GetConnection().ExecuteAsync(deleteAttributeValue, new
                                {
                                    DeviceID = obj.Device.DeviceID
                                }, trns);

                                const string attributeQuery = @"INSERT INTO ITIS_DeviceAttributeValues
                                (DeviceID,AttributeID,ValueText)
                                VALUES (@DeviceID,@AttributeID,@ValueText)";

                                foreach (var option in obj.Attributes!)
                                {
                                    await _dbConnection.GetConnection().ExecuteAsync(attributeQuery, new
                                    {
                                        DeviceID = obj.Device.DeviceID,
                                        AttributeID = option.AttributeID,
                                        ValueText = option.Value
                                    }, trns);
                                }

                                // Commit the transaction
                                trns.Commit();
                            }
                            catch (Exception)
                            {
                                // Rollback the transaction if any command fails
                                trns.Rollback();
                                throw;
                            }
                        }


                    }

                    Logdb logdb = new()
                    {
                        TrObjectId = obj.Device.DeviceID,
                        TrLog = "DEVICE UPDATED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public async Task<IEnumerable<DeviceUserDetailVM>> LoadPreviousUserDetails(int deviceID)
        {
            string sql = @"select a.AssignmentID, a.EMPNo, cm.EmpName as EMPName, a.Designation, a.AssignedDate, a.AssignRemarks, st.PropName as AssignStatus
                            , a.ApproverEMPNo, a.ApproverResponseDate, a.ApproverRemark, a.AssignDepartment as AssignDepartment, a.AssignLocation as AssignLocation
                            from ITIS_DeviceAssignments as a 
                            inner join ITIS_DeviceAssignStatus as st on st.Id=a.AssignStatusID
                            left join ITIS_MasterADEMPLOYEES as cm on cm.EmpUserName=a.EmpName
                            where a.DeviceID=@DeviceID and a.AssignStatusID=5 Order by AssignedDate desc";

            return await _dbConnection.GetConnection().QueryAsync<DeviceUserDetailVM>(sql, new
            {
                DeviceID = deviceID
            });
        }
    }
}
