using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<DeviceType> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(DeviceType deviceType)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckDeviceTypeExist(string deviceType)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_DeviceTypes
            WHERE (DeviceType = @DeviceType)";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { DeviceType = deviceType });
            return result.HasValue;
        }
    }
}
