using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class Response(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : IResponse
    {

        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<GoodsPassList>> GetList()
        {
            string sql = @"SELECT [Id], [GatePassNo], [GenDateTime], [GenGPassTo], [GpSubject], [PassStatus]  
            FROM [TGPS_VwGGPassSmry]  WHERE IsApproved = 0 AND (ApprovedById = @GenUser)
            ORDER BY GatePassNo DESC";

            return await _dbConnection.GetConnection().QueryAsync<GoodsPassList>(sql, new { GenUser = _iSessionHelper.GetUserId() });
        }

        public async Task<ShowGPListVM?> LoadShowGPDataAsync(int id)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                var sql = @"
            SELECT [Id], [GGpReference], [GpSubject], [GeneratedUser], [GeneratedDateTime],
                   [Attention], [GGPRemarks], [ApprovedBy], [ApprovedDateTime],
                   [ItemName], [ItemDescription], [Quantity], [UOM]
            FROM   [TMIS].[dbo].[TGPS_VwGatePassList] 
            WHERE  [Id] = @GPID;

            SELECT [GGpPassId], [Id], [LocationName], [ROrder], [RecGRName], [RecUser], [RecGRDateTime],
                   [RecVehicle], [RecDriver], [SendGRName], [SendUser], [SendGRDateTime], 
                   [SendVehicle], [SendDriver]
            FROM [TMIS].[dbo].[TGPS_VwGatePassRoutes] 
            WHERE [GGpPassId] = @GPID;";

                using (var multi = await connection.QueryMultipleAsync(sql, new { GPID = id }))
                {
                    var flatList = (await multi.ReadAsync<dynamic>()).ToList();
                    var routes = (await multi.ReadAsync<ShowGPRoutesVM>()).ToList();

                    if (!flatList.Any())
                        return null;

                    var g = flatList;
                    var first = g.First();

                    var result = new ShowGPListVM
                    {
                        Id = first.Id,
                        GGpReference = first.GGpReference,
                        GpSubject = first.GpSubject,
                        GeneratedUser = first.GeneratedUser,
                        GeneratedDateTime = first.GeneratedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        Attention = first.Attention,
                        GGPRemarks = first.GGPRemarks,
                        ApprovedBy = first.ApprovedBy,
                        ApprovedDateTime = first.ApprovedDateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                        ShowGPItemVMList = g.Select(i => new ShowGPItemVM
                        {
                            ItemName = i.ItemName,
                            ItemDescription = i.ItemDescription,
                            Quantity = i.Quantity?.ToString() ?? "",
                            UOM = i.UOM
                        }).ToList(),
                        ShowGPRoutesList = routes
                    };

                    return result;
                }
            }
        }

        public async Task<bool> HandleGGpAction(int id, string action)
        {
            try
            {
                string sql = @"UPDATE [dbo].[TGPS_TrGpGoodsHeader]
                   SET [IsApproved] = @Action
                      ,[ApprovedDateTime] = GETDATE()
                      ,[IsCompleted] = @IsCompleted
                 WHERE Id = @Id";
                var parameters = new { Id = id, Action = action, IsCompleted = int.Parse(action) > 1 ? 2 : 0 };
                await _dbConnection.GetConnection().ExecuteAsync(sql, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<IEnumerable<EmpPassVM>> GetEmpList()
        {
            string sql = @"SELECT  Id, [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsApproved]
            FROM            TGPS_VwEGPHeaders WHERE [IsApprovedStat] = 0  AND (ApprovedById = @GenUser)";

            return await _dbConnection.GetConnection().QueryAsync<EmpPassVM>(sql, new { GenUser = _iSessionHelper.GetUserId() });
        }

        public async Task<EmpPassVM> GetEmpPassesAsync(int id)
        {
            using var dbConnection = _dbConnection.GetConnection();

            string headerSql = @"SELECT [EmpGpNo], [GateName], [ExpLoc], [ExpReason], [ExpOutTime], [IsReturn], [IsApproved]
            FROM [TMIS].[dbo].[TGPS_VwEGPHeaders] WHERE Id = @Id;";

            // Fetch header details using Id
            var header = await dbConnection.QuerySingleOrDefaultAsync<EmpPassVM>(headerSql, new { Id = id });

            if (header == null)
            {
                throw new InvalidOperationException($"No header found for Id {id}");
            }

            // Fetch details using EGpPassId (matches EmpGpNo from header)
            var detailsSql = @"SELECT  [EGpPassId], [EmpName], [EmpEPF]
                FROM [TMIS].[dbo].[TGPS_VwEGPDetails] WHERE EGpPassId = @Id;";

            var details = await dbConnection.QueryAsync<EmpPassEmployees>(detailsSql, new { Id = id });

            header.ShowGPItemVMList = [.. details];

            return header;
        }
        public async Task<bool> HandleEGpAction(int id, string action)
        {
            try
            {
                string sql = @"UPDATE [dbo].[TGPS_TrGpEmpHeader]
                   SET [IsApproved] = @Action
                      ,[ApprovedDateTime] = GETDATE()
                 WHERE Id = @Id";
                var parameters = new { Id = id, Action = action };
                await _dbConnection.GetConnection().ExecuteAsync(sql, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
