using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class GoodsGatePass(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : IGoodsGatePass
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<GoodsPassList>> GetList()
        {
            string sql = @"SELECT [Id], [GatePassNo], [GenDateTime], [GenGPassTo], [GpSubject], [PassStatus]  
            FROM [TGPS_VwGGPassSmry] WHERE (GeneratedUserId = @GenUser)
            ORDER BY GatePassNo DESC";

            return await _dbConnection.GetConnection().QueryAsync<GoodsPassList>(sql, new { GenUser = _iSessionHelper.GetUserId() });
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
                    GpSubject,
                    GeneratedUserId,
                    GeneratedDateTime,
                    Attention,
                    ApprovedTo,
                    IsApproved,
                    IsReturnable,
                    ReturnDays,
                    GGPRemarks,
                    IsCompleted
                )
                VALUES
                (
                    @GGpReference,
                    @GpSubject,
                    @GeneratedUserId,
                    GETDATE(),
                    @Attention,
                    @ApprovedTo,
                    0,
                    @IsReturnable,
                    @ReturnDays,
                    @GGPRemarks,
                    0                    
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

                int ggpPassId = await connection.ExecuteScalarAsync<int>(
                    insertHeaderSql,
                    new
                    {
                        GGpReference = referenceNumber,
                        model.GpSubject,
                        GeneratedUserId = _iSessionHelper.GetUserId(),
                        model.Attention,
                        ApprovedTo = model.SendApprovalTo,
                        model.IsReturnable,
                        model.ReturnDays,
                        GGPRemarks = model.Remarks
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
                    GGpLocId,
                    ROrder,                 
                    IsReceived,
                    IsSend,
                    IsSender,
                    IsDest
                )
                VALUES
                (
                    @GGpPassId,
                    @GGpLocId,
                    @ROrder,
                    0,
                    0,
                    @IsSender,  
                    @IsDest
                );";


                int routeOrder = 1;
                foreach (var address in model.GatepassAddresses)
                {
                    await connection.ExecuteAsync(
                        insertRoutesSql,
                        new
                        {
                            GGpPassId = ggpPassId,
                            GGpLocId = address.LocId,
                            @IsSender = routeOrder == 1 ? 1 : 0,
                            ROrder = routeOrder++,
                            @IsDest = routeOrder == (model.GatepassAddresses.Count + 1) ? 1 : 0,
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
            WHERE        (ADMIN.dbo._MasterUsers.Id = @UserId) ORDER BY Text";

            var goodsToSql = @"SELECT Id, LocationName AS Text 
            FROM COMN_MasterTwoLocations
            UNION
            SELECT Id, AddressName AS Text 
            FROM TGPS_MasterGpGoodsAddress 
            WHERE IsDeleted = 0
            ORDER BY Text";
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
            var results = await dbConnection.QueryAsync(sql, new { UserId = _iSessionHelper.GetUserId() });
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
            var selectSql = @"SELECT TOP 1 [Id], [GenYear], [GenNo], [LastGeneratedDate]
            FROM [dbo].[TGPS_XysGenerateNumber] WHERE GenYear = @Year AND GpType='TGP'";

            var generator = await connection.QuerySingleOrDefaultAsync<dynamic>(
                selectSql, new { Year = currentYear }, transaction);

            int genNo;
            int id;

            if (generator == null)
            {
                // 2. No record for this year — insert new
                genNo = 1;

                var insertSql = @"INSERT INTO [dbo].[TGPS_XysGenerateNumber]
                    (GenYear, GenNo, LastGeneratedDate, GpType) VALUES (@GenYear, @GenNo, GETDATE(),'TEP' );
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await connection.ExecuteScalarAsync<int>(
                    insertSql,
                    new { GenYear = currentYear, GenNo = genNo + 1 },
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
                 WHERE Id = @Id AND GpType='TGP';";

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
            WHERE [GGpPassId] = @GPID;

             SELECT GpRouteId, ItemName, SendError, RecError
                    FROM TGPS_VwGatePassErrors
                    WHERE GGpPassId = @GPID;";

                using (var multi = await connection.QueryMultipleAsync(sql, new { GPID = id }))
                {
                    var flatList = (await multi.ReadAsync<dynamic>()).ToList();
                    var routes = (await multi.ReadAsync<ShowGPRoutesVM>()).ToList();
                    var errors = (await multi.ReadAsync<ShowGPListErrorsVM>()).ToList();


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
                        ShowGPRoutesList = routes,
                        ShowGPListErrorsList = errors
                    };

                    return result;
                }
            }
        }

    }
}
