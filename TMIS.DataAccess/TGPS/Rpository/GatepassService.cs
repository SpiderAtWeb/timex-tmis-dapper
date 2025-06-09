using Dapper;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class GatepassService(IDatabaseConnectionSys dbConnection) : IGatepassService
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<int> GGPUpdatAsync(string gpCode, int stat)
        {
            try
            {
                var connection = _dbConnection.GetConnection();
                int gatePassID = GetGatePassID(gpCode, connection);

                // Correct SQL for updating gatepass status
                string sql = @"UPDATE [dbo].[TGPS_TrGpGoodsHeader]
               SET  [IsApproved] = @AppStatus  
                   ,[ApprovedDateTime] = GETDATE()
                   WHERE ID = @ID";

                var parameters = new
                {
                    AppStatus = stat,
                    ID = gatePassID
                };

                return await connection.ExecuteAsync(sql, parameters);
            }
            catch 
            {
                throw;
            }
        }

        public string GetGatepassInfoAsync(string gpCode)
        {
            var connection = _dbConnection.GetConnection();
            int gatePassID = GetGatePassID(gpCode, connection);

            string sql = @"SELECT IsApproved FROM TGPS_VwGGPHeader WHERE ID = @ID";

            var gatepassInfo = connection.QueryFirstOrDefault<string>(sql, new { ID = gatePassID });
            return gatepassInfo!;
        }

        private static int GetGatePassID(string gpCode, IDbConnection connection)
        {
            const string sql = @"SELECT ID FROM TGPS_TrGpGoodsHeader WHERE GGpReference = @gpCode";
            return connection.QueryFirstOrDefault<int>(sql, new { gpCode });
        }

        public async Task<int> EGPUpdatAsync(string gpCode, int action)
        {
            try
            {
                var connection = _dbConnection.GetConnection();
                int gatePassID = GetEGatePassID(gpCode, connection);

                // Correct SQL for updating gatepass status
                string sql = @"UPDATE [dbo].[TGPS_TrGpEmpHeader]
                 SET  [IsApproved] = @AppStatus
                      ,[ApprovedDateTime] = GETDATE() 
                 WHERE ID = @ID";

                var parameters = new
                {
                    AppStatus = action,
                    ID = gatePassID
                };

                return await connection.ExecuteAsync(sql, parameters);
            }
            catch
            {
                throw;
            }
        }

        public string GetEGatepassInfoAsync(string gpCode)
        {
            var connection = _dbConnection.GetConnection();
            int gatePassID = GetEGatePassID(gpCode, connection);

            string sql = @"SELECT IsApproved FROM TGPS_VwEGPHeaders WHERE (ID = @ID)";

            var gatepassInfo = connection.QueryFirstOrDefault<string>(sql, new { ID = gatePassID });
            return gatepassInfo!;
        }

        private static int GetEGatePassID(string gpCode, IDbConnection connection)
        {
            const string sql = @"SELECT ID FROM TGPS_TrGpEmpHeader WHERE (EmpGpNo = @gpCode)";
            return connection.QueryFirstOrDefault<int>(sql, new { gpCode });
        }

    }
}
