using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class GoodsGatePass(IDatabaseConnectionSys dbConnection) : IGoodsGatePass
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<GoodsPassList>> GetList()
        {
            string sql = @"SELECT [Id], [GatePassNo], [GenDateTime], [GenGPassTo], [GpSubject], [PassStatus]  
            FROM [TMIS].[dbo].[TGPS_VwGGPassSmry] WHERE (GeneratedUserId = @GenUser)
            ORDER BY GatePassNo DESC";

            return await _dbConnection.GetConnection().QueryAsync<GoodsPassList>(sql, new { GenUser = "1" });
        }

        public async Task<string> GenerateGatePass(GatepassVM model)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            string returnRefrence = string.Empty;

            try
            {
                string referenceNumber = await GenerateGpRefAsync(connection, transaction);
                returnRefrence = referenceNumber;

                // Insert Header and get GGpPassId
                string insertHeaderSql = @"
                INSERT INTO [dbo].[TGPS_TrGpGoodsHeader]
                (
                    GGpReference,
                    GGpFromId,
                    GpSubject,
                    GeneratedUserId,
                    GeneratedDateTime,
                    Attention,
                    ApprovedTo,
                    IsApproved,
                    IsReturnable,
                    ReturnDays,
                    GGPRemarks,
                    IsCompleted,
                    IsSend,
                    IsManyLoc
                )
                VALUES
                (
                    @GGpReference,
                    @GGpFromId,
                    @GpSubject,
                    @GeneratedUserId,
                    GETDATE(),
                    @Attention,
                    @ApprovedTo,
                    0,
                    @IsReturnable,
                    @ReturnDays,
                    @GGPRemarks,
                    0,
                    0,
                    @IsManyLoc
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

                int ggpPassId = await connection.ExecuteScalarAsync<int>(
                    insertHeaderSql,
                    new
                    {
                        GGpReference = referenceNumber,
                        GGpFromId = model.GatepassAddresses.FirstOrDefault()?.From ?? string.Empty,
                        model.GpSubject,
                        GeneratedUserId = "1",
                        model.Attention,
                        ApprovedTo = model.SendApprovalTo,
                        model.IsReturnable,
                        model.ReturnDays,
                        GGPRemarks = model.Remarks,
                        IsManyLoc = model.GatepassAddresses.Count > 1 ? 1 : 0
                    },
                    transaction
                );

                // Insert Items
                string insertDetailsSql = @"
                INSERT INTO [dbo].[TGPS_TrGpGoodsDetails]
                (
                    GGpPassId,
                    ItemName,
                    ItemDescription,
                    Quantity,
                    UOMId
                )
                VALUES
                (
                    @GGpPassId,
                    @ItemName,
                    @ItemDescription,
                    @Quantity,
                    @UOMId
                ); ";

                foreach (var item in model.Items)
                {
                    await connection.ExecuteAsync(
                        insertDetailsSql,
                        new
                        {
                            GGpPassId = ggpPassId,
                            item.ItemName,
                            ItemDescription = item.ItemDesc,
                            item.Quantity,
                            UOMId = item.ItemUnit
                        },
                        transaction
                    );
                }

                // Insert Routes
                string insertRoutesSql = @"
                INSERT INTO [dbo].[TGPS_TrGpGoodsRoutes]
                (
                    GGpPassId,
                    GGpToId,
                    ROrder,                   
                    IsReceived,
                    IsCompleted
                )
                VALUES
                (
                    @GGpPassId,
                    @GGpToId,
                    @ROrder,
                    0,
                    0
                );";


                int routeOrder = 1;
                foreach (var address in model.GatepassAddresses)
                {
                    await connection.ExecuteAsync(
                        insertRoutesSql,
                        new
                        {
                            GGpPassId = ggpPassId,
                            GGpToId = address.To,
                            ROrder = routeOrder++
                        },
                        transaction
                    );
                }
                transaction.Commit();
                return returnRefrence;
            }
            catch
            {
                transaction.Rollback();
                return "Error";
            }
        }

        public async Task<GoodPassVM> GetSelectData()
        {
            var dbConnection = _dbConnection.GetConnection();
            var goodsFromSql = @"SELECT  dbo.COMN_MasterTwoLocations.Id, dbo.COMN_MasterTwoLocations.LocationName AS Text
            FROM            ADMIN.dbo._MasterUsers INNER JOIN
                                     ADMIN.dbo._TrPermissionLocation ON ADMIN.dbo._MasterUsers.Id = ADMIN.dbo._TrPermissionLocation.UserId INNER JOIN
                                     dbo.COMN_MasterTwoLocations ON ADMIN.dbo._TrPermissionLocation.LocationId = dbo.COMN_MasterTwoLocations.Id
            WHERE        (ADMIN.dbo._MasterUsers.Id = 1) ORDER BY Text";

            var goodsToSql = "SELECT Id, LocationName AS Text FROM COMN_MasterTwoLocations ORDER BY LocationName";
            var approvalListSql = "SELECT Id, UserShortName AS Text FROM [ADMIN].dbo._MasterUsers  WHERE (IsActive = 1) AND (IsGpAppUser = 1)";
            var unitsSql = "SELECT Id, PropName AS Text FROM TGPS_MasterTwoGpGoodsUOM ORDER BY PropName";

            var goodsFrom = await GetDataFromTable(goodsFromSql, dbConnection);
            var goodsTo = await GetDataFromTable(goodsToSql, dbConnection);
            var approvalList = await GetDataFromTable(approvalListSql, dbConnection);
            var units = await GetDataFromTable(unitsSql, dbConnection);

            return new GoodPassVM
            {
                GoodsFrom = goodsFrom,
                GoodsTo = goodsTo,
                ApprovalList = approvalList,
                Units = units
            };
        }

        private async Task<List<SelectListItem>> GetDataFromTable(string sql, IDbConnection dbConnection)
        {
            var results = await dbConnection.QueryAsync(sql);
            var items = results.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Text
            }).ToList();

            return items;
        }

        public async Task<string> GenerateGpRefAsync(IDbConnection connection, IDbTransaction transaction)
        {
            int currentYear = DateTime.Now.Year;

            // 1. Try to get the generator for the current year
            var selectSql = @"
            SELECT TOP 1 [Id], [GenYear], [GenNo], [LastGeneratedDate]
            FROM [dbo].[TGPS_XysGenerateNumber]
            WHERE GenYear = @Year";

            var generator = await connection.QuerySingleOrDefaultAsync<dynamic>(
                selectSql, new { Year = currentYear }, transaction);

            int genNo;
            int id;

            if (generator == null)
            {
                // 2. No record for this year — insert new
                genNo = 1;

                var insertSql = @"
                INSERT INTO [dbo].[TGPS_XysGenerateNumber]
                    (GenYear, GenNo, LastGeneratedDate)
                VALUES
                    (@GenYear, @GenNo, GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                id = await connection.ExecuteScalarAsync<int>(
                    insertSql,
                    new { GenYear = currentYear, GenNo = genNo + 1 }, // GenNo starts at 1, insert as 2 for next
                    transaction
                );
            }
            else
            {
                // 3. Record exists — increment and update
                genNo = generator.GenNo;
                id = generator.Id;

                var updateSql = @"
                UPDATE [dbo].[TGPS_XysGenerateNumber]
                SET GenNo = @NewGenNo,
                    LastGeneratedDate = GETDATE()
                WHERE Id = @Id;            ";

                await connection.ExecuteAsync(
                    updateSql,
                    new { NewGenNo = genNo + 1, Id = id },
                    transaction
                );
            }

            // 4. Format final reference number
            string reference = $"TGP/{currentYear}/{genNo.ToString("D5")}";

            return reference;
        }

        public async Task<List<GpHistoryVM>> GetHistoryData(int gpId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                var result = await connection.QueryAsync<GpHistoryVM>(
                    "TGPS_SpGpHistory",
                    new { GpId = gpId },
                    commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

    }
}
