using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceTypeRepository(IDatabaseConnectionSys dbConnection,
                            IITISLogdb iITISLogdb) : IDeviceTypeRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        public async Task<bool> AddAsync(DeviceType obj, IFormFile? image)
        {
            const string query = @"
            INSERT INTO ITIS_DeviceTypes 
            (DeviceType, Remarks, DefaultImage)
            VALUES 
            (@DeviceType, @Remarks, @DefaultImage);
            SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                byte[]? imageBytes = null;


                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    DeviceType = obj.DeviceTypeName,
                    Remarks = obj.Remarks,
                    DefaultImage = imageBytes
                });

                if (insertedId.HasValue)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "DEVICE TYPE CREATED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public async Task<bool> DeleteDeviceType(int id)
        {
            const string attributeQuery = @"Update ITIS_DeviceTypes SET
                                        IsDelete=1 where DeviceTypeID=@DeviceTypeID;";

            int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(attributeQuery, new
            {
                DeviceTypeID = id
            });
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateDeviceType(DeviceType obj, IFormFile? image)
        {

            var updateFields = new List<string>
            {
                "DeviceType = @DeviceType",
                "Remarks=@Remarks"
            };

            try
            {
                byte[]? imageBytes = null;

                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }

                    updateFields.Add("DefaultImage = @DefaultImage");
                }
                else if (obj.RemoveImage)
                {
                    updateFields.Add("DefaultImage = NULL");
                }

                var query = $@"UPDATE ITIS_DeviceTypes SET {string.Join(", ", updateFields)} where DeviceTypeID=@DeviceTypeID;";

                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    DeviceType = obj.DeviceTypeName,
                    Remarks = obj.Remarks,
                    DefaultImage = imageBytes,
                    DeviceTypeID = obj.DeviceTypeID
                });

                if (rowsAffected > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = obj.DeviceTypeID,
                        TrLog = "DEVICE TYPE UPDATED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public async Task<IEnumerable<DeviceType>> GetAllAsync()
        {
            string sql = @"SELECT 
                            DeviceTypeID,
                            DeviceType AS DeviceTypeName,
                            CreatedDate,
                            Remarks,
                            DefaultImage FROM ITIS_DeviceTypes where IsDelete = 0";

            return await _dbConnection.GetConnection().QueryAsync<DeviceType>(sql);
        }

        public async Task<bool> CheckDeviceTypeExist(string deviceType)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_DeviceTypes
            WHERE DeviceType = @DeviceType and IsDelete = 0";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { DeviceType = deviceType });
            return result.HasValue;
        }

        public async Task<bool> CheckDeviceTypeExist(DeviceType obj)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_DeviceTypes
            WHERE (DeviceType = @DeviceType and DeviceTypeID != @DeviceTypeID and IsDelete = 0)";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { DeviceType = obj.DeviceTypeName, DeviceTypeID = obj.DeviceTypeID });
            return result.HasValue;
        }

        public async Task<DeviceType?> LoadDeviceType(int id)
        {
            const string query = @"select DeviceTypeID, DeviceType as DeviceTypeName, Remarks, DefaultImage from ITIS_DeviceTypes where DeviceTypeID=@DeviceTypeID";

            var deviceType = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceType>(query, new { DeviceTypeID = id });
            return deviceType;
        }
    }
}
