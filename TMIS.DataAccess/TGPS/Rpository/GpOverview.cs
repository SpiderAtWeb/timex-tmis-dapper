using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class GpOverview(IDatabaseConnectionSys dbConnection) : IGpOverview
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<GoodsPassList>> GetList()
        {
            string sql = @"SELECT [Id], [GatePassNo], [GenDateTime], [GenGPassTo], [GpSubject], [PassStatus]  
            FROM [TGPS_VwGGPassSmry] ORDER BY GatePassNo DESC";

            return await _dbConnection.GetConnection().QueryAsync<GoodsPassList>(sql);
        }

    }
}
