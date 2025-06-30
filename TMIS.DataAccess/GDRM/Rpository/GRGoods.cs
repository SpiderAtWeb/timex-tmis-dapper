using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.GDRM.Rpository
{
    public class GRGoods(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : IGRGoods
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<GrGatepass?> GetGatepassByIdAsync(int id)
        {
            using var connection = _dbConnection.GetConnection();

            string sqlGuardRoom = @"SELECT GGpPassId FROM TGPS_TrGpGoodsRoutes WHERE (Id = @GpSubId)";
            var GatePassId = await connection.QueryFirstOrDefaultAsync<int>(sqlGuardRoom, new { GpSubId = id });

            var headerSql = @"SELECT [Id], [GpSubject], [GeneratedBy] AS GeneratedBy,[Attention], [GGPRemarks] AS Remarks, [ApprovedBy], [SendVehicleId], [SendDriverId]
            FROM [TGPS_VwGGPHeader] WHERE Id = @Id";

            var detailsSql = @"SELECT [Id], [ItemName], [ItemDescription] AS ItemDesc, [Quantity], [GpUnits]
            FROM [TGPS_VwGGPDetails] WHERE GGpPassId = @Id";

            var reasonSql = @"SELECT [Id] AS Value, [PropName] AS Text FROM [TGPS_MasterTwoGpGoodsReasons] WHERE IsDelete=0";

            var gatepass = await connection.QueryFirstOrDefaultAsync<GrGatepass>(headerSql, new { Id = GatePassId });

            if (gatepass != null)
            {
                var details = (await connection.QueryAsync<GrGatepassDetails>(detailsSql, new { Id = GatePassId })).ToList();

                var reasonList = (await connection.QueryAsync(reasonSql)).Select(r => new SelectListItem
                {
                    Value = r.Value.ToString(),
                    Text = r.Text
                }).ToList();

                foreach (var detail in details)
                {
                    detail.GrDispReasonList = reasonList;
                }

                gatepass.grGatepassDetails = details;
            }

            return gatepass;
        }

        public async Task<GPPendingListShow> GetPendingList()
        {
            using var connection = _dbConnection.GetConnection();

            // Get one guard room's info (adjust as needed — currently just takes first one)
            string sqlGuardRoom = @"SELECT [GrLocRelId], [GrName] FROM [TGPS_VwGRUsers] WHERE (Id = @UserId)";

            var oGPPendingListShow = await connection.QueryFirstOrDefaultAsync<GPPendingListShow>(sqlGuardRoom, new { UserId = _iSessionHelper.GetUserId() })
                                  ?? new GPPendingListShow();
            // Get Driver List
            string sqlDriver = @"SELECT Id AS Value, PropName AS Text 
            FROM TGPS_MasterTwoGpDrivers WHERE IsDelete = 0";

            // Get Vehicle List
            string sqlVehicles = @"SELECT Id AS Value, PropName AS Text 
            FROM TGPS_MasterTwoGpVehicles WHERE IsDelete = 0";

            // Get GP Numbers List via Stored Procedure
            var gpNumbersList = (await connection.QueryAsync<GPNumbers>(
                "TGPS_SpGPPendingList",
                new { GpLocId = oGPPendingListShow.GrLocRelId },
                commandType: CommandType.StoredProcedure)).ToList();

            var gpDriverList = (await connection.QueryAsync<SelectListItem>(sqlDriver)).ToList();
            var gpVehiclesList = (await connection.QueryAsync<SelectListItem>(sqlVehicles)).ToList();

            // Assign the rest of the data
            oGPPendingListShow.GPNumbersList = gpNumbersList;
            oGPPendingListShow.GPDriversList = gpDriverList;
            oGPPendingListShow.GPVehicleNoList = gpVehiclesList;

            return oGPPendingListShow;
        }

        public async Task<GPGrUpdateResult> GatePassUpdating(GPGrUpdate gPGrUpdate)
        {
            if (gPGrUpdate.SelectedGpId <= 0)
                return new GPGrUpdateResult { IsSuccess = false, Message = "Gatepass not found!", ErrorFieldId = "" };

            if (gPGrUpdate.ActionType != 2)
            {
                if (gPGrUpdate.VehicleNoId <= 0)
                    return new GPGrUpdateResult { IsSuccess = false, Message = "Vehicle number is required.", ErrorFieldId = "vehicleNoId" };

                if (gPGrUpdate.DriverNameId <= 0)
                    return new GPGrUpdateResult { IsSuccess = false, Message = "Driver name is required.", ErrorFieldId = "driverNameId" };
            }

            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var userId = int.Parse(_iSessionHelper.GetUserId());
                var gpId = gPGrUpdate.SelectedGpId;

                if (gPGrUpdate.IsOut > 0)
                {
                    if (gPGrUpdate.ActionType == 2)
                    {
                        await UpdateRouteAsync(connection, transaction, gpId, 2, gPGrUpdate.GRId, userId, true);
                        await MarkHeaderCompletedAsync(connection, transaction, gpId, 2);
                    }
                    else
                    {
                        if (await IsExternalAsync(connection, transaction, gpId))
                            await MarkHeaderCompletedAsync(connection, transaction, gpId, 1);

                        await UpdateRouteWithDetailsAsync(connection, transaction, gPGrUpdate, userId, true);
                        await InsertDetailsAsync(connection, transaction, gPGrUpdate, true);
                    }
                }
                else
                {
                    if (gPGrUpdate.ActionType == 2)
                    {
                        await UpdateRouteAsync(connection, transaction, gpId, 2, gPGrUpdate.GRId, userId, false);
                        await MarkHeaderCompletedAsync(connection, transaction, gpId, 2);
                    }
                    else
                    {
                        var destinationInfo = await GetDestinationInfoAsync(connection, transaction, gpId);

                        if (destinationInfo!.IsDest && !destinationInfo.IsExternal)
                            await MarkHeaderCompletedAsync(connection, transaction, gpId, 1);

                        await UpdateRouteWithDetailsAsync(connection, transaction, gPGrUpdate, userId, false);
                        await InsertDetailsAsync(connection, transaction, gPGrUpdate, false);
                    }
                }

                transaction.Commit();
                return new GPGrUpdateResult { IsSuccess = true, Message = "Transaction Completed Successfully." };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new GPGrUpdateResult { IsSuccess = false, Message = "An error occurred: " + ex.Message };
            }
        }

        private async Task<bool> IsExternalAsync(IDbConnection connection, IDbTransaction transaction, int gpId)
        {
            const string query = @"SELECT TGPS_TrGpGoodsHeader.IsExternal
                           FROM TGPS_TrGpGoodsHeader
                           INNER JOIN TGPS_TrGpGoodsRoutes ON TGPS_TrGpGoodsHeader.ID = TGPS_TrGpGoodsRoutes.GGpPassId
                           WHERE TGPS_TrGpGoodsRoutes.ID = @GpId";

            return await connection.QueryFirstOrDefaultAsync<bool>(query, new { GpId = gpId }, transaction);
        }

        private async Task<CompleteTrip?> GetDestinationInfoAsync(IDbConnection connection, IDbTransaction transaction, int gpId)
        {
            const string query = @"SELECT TGPS_TrGpGoodsHeader.IsExternal, TGPS_TrGpGoodsRoutes.IsDest
                           FROM TGPS_TrGpGoodsHeader
                           INNER JOIN TGPS_TrGpGoodsRoutes ON TGPS_TrGpGoodsHeader.ID = TGPS_TrGpGoodsRoutes.GGpPassId
                           WHERE TGPS_TrGpGoodsRoutes.ID = @GpId";

            return await connection.QueryFirstOrDefaultAsync<CompleteTrip>(query, new { GpId = gpId }, transaction);
        }

        private async Task UpdateRouteAsync(IDbConnection connection, IDbTransaction transaction, int gpId, int status, int grId, int userId, bool isSend)
        {
            string sql = isSend
                ? @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
              SET [IsSend] = @Status,
                  [SendGRId] = @GRId,
                  [SendGRUserId] = @UserId,
                  [SendGRDateTime] = GETDATE()
            WHERE [Id] = @GpId"
                : @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
              SET [IsReceived] = @Status,
                  [RecGRId] = @GRId,
                  [RecGRUserId] = @UserId,
                  [RecGRDateTime] = GETDATE()
            WHERE [Id] = @GpId";

            await connection.ExecuteAsync(sql, new { GpId = gpId, Status = status, GRId = grId, UserId = userId }, transaction);
        }

        private async Task UpdateRouteWithDetailsAsync(IDbConnection connection, IDbTransaction transaction, GPGrUpdate gPGrUpdate, int userId, bool isSend)
        {
            string sql = isSend
                ? @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
              SET [IsSend] = 1,
                  [SendGRId] = @GRId,
                  [SendGRUserId] = @UserId,
                  [SendGRDateTime] = GETDATE(),
                  [SendVehicleId] = @VehicleId,
                  [SendDriverId] = @DriverId
            WHERE [Id] = @GpId"
                : @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
              SET [IsReceived] = 1,
                  [RecGRId] = @GRId,
                  [RecGRUserId] = @UserId,
                  [RecGRDateTime] = GETDATE(),
                  [RecVehicleId] = @VehicleId,
                  [RecDriverId] = @DriverId
            WHERE [Id] = @GpId";

            await connection.ExecuteAsync(sql, new
            {
                GpId = gPGrUpdate.SelectedGpId,
                GRId = gPGrUpdate.GRId,
                UserId = userId,
                VehicleId = gPGrUpdate.VehicleNoId,
                DriverId = gPGrUpdate.DriverNameId
            }, transaction);
        }

        private async Task InsertDetailsAsync(IDbConnection connection, IDbTransaction transaction, GPGrUpdate gPGrUpdate, bool isSend)
        {
            string sql = isSend
                ? @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsErrorsSend]
              ([GpRouteId], [GGpPassDetailsId], [GRRemarkId], [ActualQty])
            VALUES
              (@GpRouteId, @DetailsId, @RemarkId, @ActualQty)"
                : @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsErrorsRec]
              ([GpRouteId], [GGpPassDetailsId], [GRRemarkId], [ActualQty])
            VALUES
              (@GpRouteId, @DetailsId, @RemarkId, @ActualQty)";

            foreach (var detail in gPGrUpdate.GPGrUpdateDetailList)
            {
                await connection.ExecuteAsync(sql, new
                {
                    GpRouteId = gPGrUpdate.SelectedGpId,
                    DetailsId = detail.ID,
                    RemarkId = detail.ReasonId,
                    detail.ActualQty
                }, transaction);
            }
        }

        private async Task MarkHeaderCompletedAsync(IDbConnection connection, IDbTransaction transaction, int gpId, int status)
        {
            const string sql = @"UPDATE [dbo].[TGPS_TrGpGoodsHeader]
                         SET [IsCompleted] = @Status
                         FROM TGPS_TrGpGoodsHeader
                         INNER JOIN TGPS_TrGpGoodsRoutes ON TGPS_TrGpGoodsHeader.ID = TGPS_TrGpGoodsRoutes.GGpPassId
                         WHERE TGPS_TrGpGoodsRoutes.ID = @GpId";

            await connection.ExecuteAsync(sql, new { GpId = gpId, Status = status }, transaction);
        }


        public async Task<List<GpHistoryVM>> GetGDHistoryData(int gpId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                string sqlGuardRoom = @"SELECT GGpPassId FROM TGPS_TrGpGoodsRoutes WHERE (Id = @GpSubId)";
                var GatePassId = await connection.QueryFirstOrDefaultAsync<int>(sqlGuardRoom, new { GpSubId = gpId });

                var result = await connection.QueryAsync<GpHistoryVM>(
                    "TGPS_SpGpHistory",
                    new { GpId = GatePassId },
                    commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }
    }
}
