using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class GoodsGatePass(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper, IUserControls userControls, IGmailSender gmailSender) : IGoodsGatePass
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IUserControls _userControls = userControls;
        private readonly IGmailSender _gmailSender = gmailSender;

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
                string referenceNumber = await _userControls.GenerateGpRefAsync(connection, transaction, "[TGPS_XysGenerateNumber]", "TGP");
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
                    ApprovedById,
                    IsApproved,
                    IsReturnable,
                    ReturnDays,
                    GGPRemarks,
                    IsCompleted,
                    BoiGatepass,
                    IsExternal 
                )
                VALUES
                (
                    @GGpReference,
                    @GpSubject,
                    @GeneratedUserId,
                    GETDATE(),
                    @Attention,
                    @ApprovedById,
                    0,
                    @IsReturnable,
                    @ReturnDays,
                    @GGPRemarks,
                    0,
                    @BoiGatepass,
                    @IsExternal
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
                        ApprovedById = model.SendApprovalTo,
                        model.IsReturnable,
                        model.ReturnDays,
                        GGPRemarks = model.Remarks,
                        model.BoiGatepass,
                        model.IsExternal
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

                PrepairEmail(ggpPassId);
                return returnRefrence;
            }
            catch
            {
                transaction.Rollback();
                return "Error";
            }
        }
        private void PrepairEmail(int genId)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"
            SELECT 
            GGpReference, 
            GpSubject, 
            GeneratedDateTime, 
            Attention, 
            GGPRemarks, 
            GeneratedBy,
            (
                 SELECT STRING_AGG(LOC.BusinessName, ' => ')
                 FROM dbo.TGPS_TrGpGoodsRoutes AS RT
                 INNER JOIN dbo.TGPS_MasterGpGoodsAddress AS LOC ON RT.GGpLocId = LOC.Id
                WHERE RT.GGpPassId = H.Id
            ) AS GpTo,
            ApprovedById
            FROM TGPS_VwGGPHeader AS H
            WHERE Id = @genId;            ";

            var header = connection.Query(headerQuery, new { GenId = genId }).FirstOrDefault();

            string detailsQuery = @"
            SELECT ItemName, ItemDescription, Quantity, GpUnits 
            FROM   TGPS_VwGGPDetails
            WHERE  (GGpPassId = @genId)";

            var details = connection.Query(detailsQuery, new { GenId = genId }).ToList();

            // Prepare header part of array
            var myList = new List<string>
            {
               $"{header!.GGpReference}",
                $"{header.GpSubject}",
                $"{header.GeneratedDateTime}",
                $"{header.Attention}",
                $"{header.GGPRemarks}",
                $"{header.GeneratedBy}",
                $"{header.GpTo}"
            };

            // Append each detail row as a string item in the array
            foreach (var d in details)
            {
                string detailString = $"{d.ItemName}|{d.ItemDescription}|{d.Quantity}|{d.GpUnits}";
                myList.Add(detailString);
            }

            // Convert to array
            string[] myArray = [.. myList];

            string approveByMail = connection.Query<string>("SELECT UserEmail FROM ADMIN.dbo._MasterUsers WHERE Id = @Id",
            new { Id = header.ApprovedById }).FirstOrDefault() ?? throw new InvalidOperationException("No email found for the approved user."); ;

            // Send email
            Task.Run(() => _gmailSender.GPRequestToApprove(approveByMail, myArray));
        }

        public async Task<GoodPassVM> GetFillData(bool isExternal)
        {
            var dbConnection = _dbConnection.GetConnection();


            var goodsFromSql = @"SELECT Id, Text FROM TGPS_VwGPUserLocs WHERE (UserId = @UserId)";
            var goodsFrom = await GetDataFromTable(goodsFromSql, dbConnection);

            // Extract goodsFrom IDs
            var goodsFromIds = goodsFrom.Select(x => x.Value).ToList();

            // Step 2: Prepare the goodsTo SQL based on isExternal
            string goodsToSqlBase = @"SELECT Id, BusinessName AS Text FROM TGPS_MasterGpGoodsAddress 
                          WHERE IsDeleted = 0 AND (IsExternal = @IsExternal)";

            if (goodsFromIds.Any())
            {
                // Create a comma-separated string of IDs to exclude
                string excludedIds = string.Join(",", goodsFromIds);
                goodsToSqlBase += $" AND Id NOT IN ({excludedIds})";
            }

            goodsToSqlBase += " ORDER BY Text";

            // Execute with isExternal parameter
            //var goodsTo = await GetDataFromTable(goodsToSqlBase, dbConnection, new { IsExternal = isExternal });

            var results = await dbConnection.QueryAsync(goodsToSqlBase, new { IsExternal = isExternal });
            var goodsTo = results.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Text
            }).ToList();

            var approvalListSql = @"SELECT AppUserId AS Id, UserShortName AS Text
                        FROM ADMIN.dbo.TGPS_VwUserApprovePersons 
                        WHERE (UserId = @UserId) AND (SystemType = N'TGP')";

            var unitsSql = "SELECT Id, PropName AS Text FROM TGPS_MasterTwoGpGoodsUOM ORDER BY PropName";

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
                       [ItemName], [ItemDescription], [Quantity], [UOM], [IsApproved], [BoiGatepass]
                FROM   [TMIS].[dbo].[TGPS_VwGatePassList] 
                WHERE  [Id] = @GPID;

                SELECT [GGpPassId], [Id], [LocationName], [ROrder], [RecGRName], [RecUser], [RecGRDateTime],
                       [RecVehicle], [RecDriver], [SendGRName], [SendUser], [SendGRDateTime], 
                       [SendVehicle], [SendDriver]
                FROM [TMIS].[dbo].[TGPS_VwGatePassRoutes] 
                WHERE [GGpPassId] = @GPID;

             SELECT GpRouteId, ItemName, SendError, RecError, SendQty, RecQty
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
                        IsApproved = first.IsApproved,
                        BoiGatepass = first.BoiGatepass,
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
