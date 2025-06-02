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

            var headerSql = @"SELECT [Id], [GpSubject], [GeneratedBy] AS GeneratedBy,[Attention], [GGPRemarks] AS Remarks, [ApprovedBy]
            FROM [TGPS_VwGGPHeader] WHERE Id = @Id";

            var detailsSql = @"SELECT [Id], [ItemName], [ItemDescription] AS ItemDesc,[Quantity], [GpUnits]
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
            string sqlGuardRoom = @"SELECT A.Id AS GRId, B.PropName AS GRName, dbo.COMN_MasterTwoLocations.Id AS GRLocationId, 
                         dbo.COMN_MasterTwoLocations.LocationName AS GRLocation
            FROM            [ADMIN].dbo.GTPS_RelGRoomsLoc AS A INNER JOIN
                                     [ADMIN].dbo.GTPS_MasterGRooms AS B ON A.GuardRoomId = B.Id INNER JOIN
                                     dbo.COMN_MasterTwoLocations ON A.LocationId = dbo.COMN_MasterTwoLocations.Id INNER JOIN
                                     ADMIN.dbo._MasterUsers AS U ON A.Id = U.DefGRoomId
            WHERE        (U.Id = @UserId)";

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
                new { GpLocId = oGPPendingListShow.GRLocationId },
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
                return await Task.FromResult(new GPGrUpdateResult { IsSuccess = false, Message = "Gatepass not found!", ErrorFieldId = "" });

            if (gPGrUpdate.VehicleNoId <= 0)
                return await Task.FromResult(new GPGrUpdateResult { IsSuccess = false, Message = "Vehicle number is required.", ErrorFieldId = "vehicleNoId" });

            if (gPGrUpdate.DriverNameId <= 0)
                return await Task.FromResult(new GPGrUpdateResult { IsSuccess = false, Message = "Driver name is required.", ErrorFieldId = "driverNameId" });

            var connection = _dbConnection.GetConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                if (gPGrUpdate.IsOut > 0)
                {
                    // Update from sql query
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
                        SET    [IsSend] = @IsSend
                              ,[SendGRId] = @GRId
                              ,[SendGRUserId] = @SendGRUserId
                              ,[SendGRDateTime] = GETDATE() 
                              ,[SendVehicleId] = @VehicleNoId
                              ,[SendDriverId] = @DriverNameId
                        WHERE [Id] = @GpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            GpId = gPGrUpdate.SelectedGpId,
                            IsSend = gPGrUpdate.ActionType ? 1 : 2,
                            gPGrUpdate.GRId,
                            SendGRUserId = _iSessionHelper.GetUserId(),
                            gPGrUpdate.VehicleNoId,
                            gPGrUpdate.DriverNameId
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //insert the dispatching details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsErrorsSend]
                           ([GpRouteId]
                           ,[GGpPassDetailsId]
                           ,[GRRemarkId])
                     VALUES
                           (@GpRouteId
                           ,@GGpPassDetailsId
                           ,@GuardRoomRemarkId)";

                    foreach (var detail in gPGrUpdate.GPGrUpdateDetailList)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
                                GpRouteId = gPGrUpdate.SelectedGpId,
                                GGpPassDetailsId = detail.ID,
                                GuardRoomRemarkId = detail.ReasonId

                            },
                            transaction: transaction,
                            commandType: CommandType.Text);
                    }
                }
                else
                {
                    // Update from sql query
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpGoodsRoutes]
                       SET [IsReceived] = @IsReceived                         
                          ,[RecGRId] = @GRId
                          ,[RecGRUserId] = @RecGRUserId
                          ,[RecGRDateTime] = GETDATE()  
                          ,[RecVehicleId] = @VehicleNoId
                          ,[RecDriverId] = @DriverNameId
                     WHERE [Id] = @GpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            GpId = gPGrUpdate.SelectedGpId,
                            IsReceived = gPGrUpdate.ActionType ? 1 : 2,
                            gPGrUpdate.GRId,
                            RecGRUserId = _iSessionHelper.GetUserId(),
                            gPGrUpdate.VehicleNoId,
                            gPGrUpdate.DriverNameId
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //insert the dispatching details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsErrorsRec]
                           ([GpRouteId]
                           ,[GGpPassDetailsId]
                           ,[GRRemarkId])
                     VALUES
                           (@GpRouteId
                           ,@GGpPassDetailsId
                           ,@GuardRoomRemarkId)";

                    foreach (var detail in gPGrUpdate.GPGrUpdateDetailList)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
                                GpRouteId = gPGrUpdate.SelectedGpId,
                                GGpPassDetailsId = detail.ID,
                                GuardRoomRemarkId = detail.ReasonId

                            },
                            transaction: transaction,
                            commandType: CommandType.Text);
                    }
                }

                transaction.Commit();
                return await Task.FromResult(new GPGrUpdateResult { IsSuccess = true, Message = "Dispatching completed successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Task.FromResult(new GPGrUpdateResult { IsSuccess = false, Message = "An error occurred: " + ex.Message });
            }
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
