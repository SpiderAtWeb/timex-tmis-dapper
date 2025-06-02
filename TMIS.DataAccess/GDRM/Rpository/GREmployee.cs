using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.Models.GDRM;

namespace TMIS.DataAccess.GDRM.Rpository
{
    public class GREmployee(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : IGREmployee
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<EmpGatepass?> GetEmployeeGatepassByIdAsync(int id)
        {
            using var connection = _dbConnection.GetConnection();

            var headerSql = @"SELECT Id, EmpGpNo, GateName, ExpLoc, ExpReason, ExpOutTime, IsReturn, ResponsedDateTime, ResponsedBy, GenUser
            FROM TGPS_VwEGPHeaders WHERE (Id = @Id)";

            var detailsSql = @"SELECT [Id], [EmpName], [EmpEPF]
                          FROM [TGPS_VwEGPDetails] WHERE EGpPassId = @Id";

            var empGatepass = await connection.QueryFirstOrDefaultAsync<EmpGatepass>(headerSql, new { Id = id });

            if (empGatepass != null)
            {
                var details = (await connection.QueryAsync<EmpGatepassDetails>(detailsSql, new { Id = id })).ToList();
                empGatepass.EmpGatepassDetails = details;
            }
            return empGatepass;
        }

        public async Task<EmpPendingListShow> GetEmployeePendingList()
        {
            using var connection = _dbConnection.GetConnection();

            // Get guard room info
            string sqlGuardRoom = @"SELECT A.Id AS GRId, B.PropName AS GRName, dbo.COMN_MasterTwoLocations.Id AS GRLocationId, 
                               dbo.COMN_MasterTwoLocations.LocationName AS GRLocation
                               FROM [ADMIN].dbo.GTPS_RelGRoomsLoc AS A 
                               INNER JOIN [ADMIN].dbo.GTPS_MasterGRooms AS B ON A.GuardRoomId = B.Id 
                               INNER JOIN dbo.COMN_MasterTwoLocations ON A.LocationId = dbo.COMN_MasterTwoLocations.Id 
                               INNER JOIN ADMIN.dbo._MasterUsers AS U ON A.Id = U.DefGRoomId
                               WHERE (U.Id = @UserId)";

            var oEmpPendingListShow = await connection.QueryFirstOrDefaultAsync<EmpPendingListShow>(sqlGuardRoom, new { UserId = _iSessionHelper.GetUserId() })
                                     ?? new EmpPendingListShow();

            // Get Driver List (reusing the same drivers from goods pass)
            string sqlDriver = @"SELECT Id AS Value, PropName AS Text 
                            FROM TGPS_MasterTwoGpDrivers WHERE IsDelete = 0";

            // Get Vehicle List (reusing the same vehicles from goods pass)
            string sqlVehicles = @"SELECT Id AS Value, PropName AS Text 
                              FROM TGPS_MasterTwoGpVehicles WHERE IsDelete = 0";

            // Get Employee GP Numbers List via Stored Procedure
            var empNumbersList = (await connection.QueryAsync<EmpNumbers>(
                "TGPS_SpEmpGPPendingList", // You'll need to create this stored procedure
                new { EmpGpLocId = oEmpPendingListShow.GRLocationId },
                commandType: CommandType.StoredProcedure)).ToList();

            var empDriverList = (await connection.QueryAsync<SelectListItem>(sqlDriver)).ToList();
            var empVehiclesList = (await connection.QueryAsync<SelectListItem>(sqlVehicles)).ToList();

            // Assign the data
            oEmpPendingListShow.EmpNumbersList = empNumbersList;
            oEmpPendingListShow.EmpDriversList = empDriverList;
            oEmpPendingListShow.EmpVehicleNoList = empVehiclesList;

            return oEmpPendingListShow;
        }

        public async Task<EmpGpUpdateResult> EmployeeGatePassUpdating(EmpGpUpdate empGpUpdate)
        {
            if (empGpUpdate.SelectedEmpGpId <= 0)
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = false, Message = "Employee gatepass not found!", ErrorFieldId = "" });

            if (empGpUpdate.VehicleNoId <= 0)
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = false, Message = "Vehicle number is required.", ErrorFieldId = "vehicleNoId" });

            if (empGpUpdate.DriverNameId <= 0)
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = false, Message = "Driver name is required.", ErrorFieldId = "driverNameId" });

            var connection = _dbConnection.GetConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                if (empGpUpdate.IsOut > 0)
                {
                    // Update for outgoing
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpEmpRoutes]
                                     SET [IsSend] = @IsSend,
                                         [SendGRId] = @GRId,
                                         [SendGRUserId] = @SendGRUserId,
                                         [SendGRDateTime] = GETDATE(),
                                         [SendVehicleId] = @VehicleNoId,
                                         [SendDriverId] = @DriverNameId
                                     WHERE [Id] = @EmpGpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            EmpGpId = empGpUpdate.SelectedEmpGpId,
                            IsSend = empGpUpdate.ActionType ? 1 : 2,
                            empGpUpdate.GRId,
                            SendGRUserId = _iSessionHelper.GetUserId(),
                            empGpUpdate.VehicleNoId,
                            empGpUpdate.DriverNameId
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    // Insert dispatching details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpEmpDetailsErrorsSend]
                                     ([EmpRouteId], [EGpPassDetailsId], [GRRemarkId])
                                     VALUES (@EmpRouteId, @EGpPassDetailsId, @GuardRoomRemarkId)";

                    foreach (var detail in empGpUpdate.EmpGpUpdateDetailList)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
                                EmpRouteId = empGpUpdate.SelectedEmpGpId,
                                EGpPassDetailsId = detail.ID,
                                GuardRoomRemarkId = detail.ReasonId
                            },
                            transaction: transaction,
                            commandType: CommandType.Text);
                    }
                }
                else
                {
                    // Update for receiving
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpEmpRoutes]
                                     SET [IsReceived] = @IsReceived,
                                         [RecGRId] = @GRId,
                                         [RecGRUserId] = @RecGRUserId,
                                         [RecGRDateTime] = GETDATE(),
                                         [RecVehicleId] = @VehicleNoId,
                                         [RecDriverId] = @DriverNameId
                                     WHERE [Id] = @EmpGpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            EmpGpId = empGpUpdate.SelectedEmpGpId,
                            IsReceived = empGpUpdate.ActionType ? 1 : 2,
                            empGpUpdate.GRId,
                            RecGRUserId = _iSessionHelper.GetUserId(),
                            empGpUpdate.VehicleNoId,
                            empGpUpdate.DriverNameId
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    // Insert receiving details
                    string sqlInsertM = @"INSERT INTO [dbo].[TGPS_TrGpEmpDetailsErrorsRec]
                                     ([EmpRouteId], [EGpPassDetailsId], [GRRemarkId])
                                     VALUES (@EmpRouteId, @EGpPassDetailsId, @GuardRoomRemarkId)";

                    foreach (var detail in empGpUpdate.EmpGpUpdateDetailList)
                    {
                        connection.Execute(
                            sqlInsertM,
                            new
                            {
                                EmpRouteId = empGpUpdate.SelectedEmpGpId,
                                EGpPassDetailsId = detail.ID,
                                GuardRoomRemarkId = detail.ReasonId
                            },
                            transaction: transaction,
                            commandType: CommandType.Text);
                    }
                }

                transaction.Commit();
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = true, Message = "Employee dispatching completed successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = false, Message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<List<EmpHistoryVM>> GetEmpHistoryData(int empGpId)
        {
            using var connection = _dbConnection.GetConnection();

            string sqlEmpGuardRoom = @"SELECT EGpPassId FROM TGPS_TrGpEmpRoutes WHERE (Id = @EmpGpSubId)";
            var empGatePassId = await connection.QueryFirstOrDefaultAsync<int>(sqlEmpGuardRoom, new { EmpGpSubId = empGpId });

            var result = await connection.QueryAsync<EmpHistoryVM>(
                "TGPS_SpEmpGpHistory", // You'll need to create this stored procedure
                new { EmpGpId = empGatePassId },
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
    }
}
