using Dapper;
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
            const string sql = @"SELECT [Id], [BusinessName], [Address], [City], [State], [Phone]
                             FROM [TMIS].[dbo].[TGPS_MasterGpGoodsAddress] WHERE IsDeleted = 0 AND (IsExternal = 1)";
            using var connection = _dbConnection.GetConnection();
            return await connection.QueryAsync<AddressModel>(sql);
        }

        public async Task<AddressModel?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT [Id],  [BusinessName], [Address], [City], [State], [Phone]
                             FROM [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                             WHERE Id = @Id AND IsDeleted = 0";
            using var connection = _dbConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<AddressModel>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(AddressModel model)
        {
            try
            {
                model.Phone = string.Concat(model.Phone.Where(c => !char.IsWhiteSpace(c)));

                const string checkSql = @"SELECT COUNT(1) FROM [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                              WHERE BusinessName = @BusinessName AND IsDeleted = 0";

                using var connection = _dbConnection.GetConnection();

                var exists = await connection.ExecuteScalarAsync<int>(checkSql, new { model.BusinessName });

                if (exists > 0)
                {
                    throw new InvalidOperationException("BusinessName already exists.");
                }

                const string insertSql = @"INSERT INTO [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                               ([BusinessName], [Address], [City], [State], [Phone], IsDeleted, IsExternal)
                               VALUES (@BusinessName, @Address, @City, @State, @Phone, 0, 1);
                               SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.ExecuteScalarAsync<int>(insertSql, model);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> UpdateAsync(AddressModel model)
        {
            const string sql = @"UPDATE [TMIS].[dbo].[TGPS_MasterGpGoodsAddress]
                             SET BusinessName = @BusinessName,
                                 Address = @Address,
                                 City = @City,
                                 State = @State,
                                 Phone = @Phone
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
