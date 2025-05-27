using Dapper;
using System.Data.Common;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class AddressBank(IDatabaseConnectionSys dbConnection) : IAddressBank
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<AddressModel>> GetAllAsync()
        {
            const string sql = @"SELECT [Id], [AddressName], [AddressAddressLane1], [AddressAddressLane2], [AddressAddressLane3], [ContactNos]
                             FROM [TMIS].[dbo].[TGPS_MasterGpGoodsAddress] WHERE IsDeleted = 0";
            using var connection = _dbConnection.GetConnection();
            return await connection.QueryAsync<AddressModel>(sql);
        }

        public async Task<AddressModel?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT [Id], [AddressName], [AddressAddressLane1], [AddressAddressLane2], [AddressAddressLane3], [ContactNos]
                             FROM [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                             WHERE Id = @Id AND IsDeleted = 0";
            using var connection = _dbConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<AddressModel>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(AddressModel model)
        {
            const string sql = @"INSERT INTO [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                            ([AddressName], [AddressAddressLane1], [AddressAddressLane2], [AddressAddressLane3], [ContactNos], IsDeleted)
                            VALUES (@AddressName, @AddressAddressLane1, @AddressAddressLane2, @AddressAddressLane3, @ContactNos, 0);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _dbConnection.GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, model);
        }

        public async Task<int> UpdateAsync(AddressModel model)
        {
            const string sql = @"UPDATE [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                             SET AddressName = @AddressName,
                                 AddressAddressLane1 = @AddressAddressLane1,
                                 AddressAddressLane2 = @AddressAddressLane2,
                                 AddressAddressLane3 = @AddressAddressLane3,
                                 ContactNos = @ContactNos
                             WHERE Id = @Id";

            using var connection = _dbConnection.GetConnection();
            return await connection.ExecuteAsync(sql, model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string sql = @"UPDATE [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                             SET IsDeleted = 1
                             WHERE Id = @Id";

            using var connection = _dbConnection.GetConnection();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

}
