using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;

namespace TMIS.DataAccess.GDRM.Rpository
{
    public class GRGoods(IDatabaseConnectionSys dbConnection) : IGRGoods
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<GrGatepass?> GetGatepassByIdAsync(int id)
        {
            var headerSql = @"
            SELECT [Id], [GpSubject], [GeneratedUserId] AS GeneratedBy,
                   [Attention], [GGPRemarks] AS Remarks
            FROM [TMIS].[dbo].[TGPS_VwGGPHeader]
            WHERE Id = @Id";

            var detailsSql = @"
            SELECT [Id], [ItemName], [ItemDescription] AS ItemDesc,
                   [Quantity], [GpUnits]
            FROM [TMIS].[dbo].[TGPS_VwGGPDetails]
            WHERE GGpPassId = @Id";

            var reasonSql = @"
            SELECT [Id] AS Value, [PropName] AS Text FROM [TMIS].[dbo].[TGPS_MasterTwoGpGoodsReasons]";

            var connection = _dbConnection.GetConnection();

            var gatepass = await connection.QueryFirstOrDefaultAsync<GrGatepass>(headerSql, new { Id = id });

            if (gatepass != null)
            {
                var details = (await connection.QueryAsync<GrGatepassDetails>(detailsSql, new { Id = id })).ToList();

                var reasonList = (await connection.QueryAsync(reasonSql)).Select(r => new SelectListItem
                {
                    Value = r.Value.ToString(),
                    Text = r.Text
                }).ToList();

                foreach (var detail in details)
                {
                    detail.GrDispReasonList = reasonList;
                }

                gatepass.DpDetailsList = details;
            }

            return gatepass;
        }



        public async Task<GPDispatchShow> GetDispachingList()
        {
            var oGPDispatchShow = new GPDispatchShow
            {
                GRId = 3,
                GRName = "G.ROOM FRONT",
                GRLocation = "UNIT-03"
            };

            string sqlDriver = @"
            SELECT   Id AS Value, PropName AS Text
            FROM TGPS_MasterTwoGpDrivers WHERE (IsDelete = 0)";

            string sqlVehicles = @"
             SELECT   Id AS Value, PropName AS Text
            FROM TGPS_MasterTwoGpVehicles WHERE (IsDelete = 0)";

            int id = 3; //
            var gpNumbersList = (await _dbConnection.GetConnection()
                .QueryAsync<GPNumbers>("TGPS_SpDispatchList", new { GpLocId = id }, commandType: CommandType.StoredProcedure))
                .ToList();
            var gpDriverList = (await _dbConnection.GetConnection().QueryAsync<SelectListItem>(sqlDriver)).ToList();
            var gpVehiclesList = (await _dbConnection.GetConnection().QueryAsync<SelectListItem>(sqlVehicles)).ToList();

            oGPDispatchShow.GPNumbersList = gpNumbersList;
            oGPDispatchShow.GPDriversList = gpDriverList;
            oGPDispatchShow.GPVehicleNoList = gpVehiclesList;

            return oGPDispatchShow;
        }

        public async Task<DispatchResult> DispatchingGoods(Dispatching dispatch)
        {
            if (dispatch.SelectedGpId <= 0)
                return await Task.FromResult(new DispatchResult { IsSuccess = false, Message = "Gatepass not found!", ErrorFieldId = "" });

            if (dispatch.VehicleNoId <= 0)
                return await Task.FromResult(new DispatchResult { IsSuccess = false, Message = "Vehicle number is required.", ErrorFieldId = "vehicleNoId" });

            if (dispatch.DriverNameId <= 0)
                return await Task.FromResult(new DispatchResult { IsSuccess = false, Message = "Driver name is required.", ErrorFieldId = "driverNameId" });


            var connection = _dbConnection.GetConnection();
            var transaction = connection.BeginTransaction();

            try
            {

                if (dispatch.SelectedGpIdType > 0)
                {
                    // Update from sql query
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpGoodsHeader]
                      SET  [IsSend] = @IsSend
                          ,[SendGRId] = 1
                          ,[SendGRUserId] = 1
                          ,[SendGRDateTime] = GETDATE()
                          ,[VehicleId] = @VehicleNoId
                          ,[DriverId] = @DriverNameId
                     WHERE Id=@GpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            GpId = dispatch.SelectedGpId,
                            @IsSend = dispatch.ActionType ? 1 : 0,
                            dispatch.VehicleNoId,
                            dispatch.DriverNameId
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //insert the dispatching details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsValidations]
                       ([LocationId]
                       ,[GGpPassDetailsId]
                       ,[GuardRoomRemarkId])
                        VALUES
                       (1
                       ,@GGpPassDetailsId
                       ,@GuardRoomRemarkId)";

                    foreach (var detail in dispatch.DispatchingDetails)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
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
                       SET  [IsSend] = @IsSend
                           ,[SendGRId] = 1
                           ,[SendGRUserId] = 1
                           ,[SendGRDateTime] =  GETDATE()
                     WHERE Id=@GpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            @IsSend = dispatch.ActionType ? 1 : 0,
                            GpId = dispatch.SelectedGpId    
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //insert the dispatching details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpGoodsDetailsValidations]
                       ([LocationId]
                       ,[GGpPassDetailsId]
                       ,[GuardRoomRemarkId])
                        VALUES
                       (1
                       ,@GGpPassDetailsId
                       ,@GuardRoomRemarkId)";

                    foreach (var detail in dispatch.DispatchingDetails)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
                                GGpPassDetailsId = detail.ID,
                                GuardRoomRemarkId = detail.ReasonId
                            },
                            transaction: transaction,
                            commandType: CommandType.Text);
                    }

                }

                transaction.Commit();
                return await Task.FromResult(new DispatchResult { IsSuccess = true, Message = "Dispatching completed successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Task.FromResult(new DispatchResult { IsSuccess = false, Message = "An error occurred: " + ex.Message });
            }

        }
    }
}
