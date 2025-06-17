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
            string sqlGuardRoom = @"SELECT [GrLocRelId], [GrName] FROM [TGPS_VwGRUsers] WHERE (Id = @UserId)";

            var oEmpPendingListShow = await connection.QueryFirstOrDefaultAsync<EmpPendingListShow>(sqlGuardRoom, new { UserId = _iSessionHelper.GetUserId() })
                                     ?? new EmpPendingListShow();

            // Get Employee GP Numbers List via Stored Procedure
            var empNumbersList = (await connection.QueryAsync<EmpNumbers>(
                "TGPS_SpEmpGPPendingList", // You'll need to create this stored procedure
                new { EmpGpLocId = oEmpPendingListShow.GrLocRelId },
                commandType: CommandType.StoredProcedure)).ToList();

            // Assign the data
            oEmpPendingListShow.EmpNumbersList = empNumbersList;

            return oEmpPendingListShow;
        }

        public async Task<EmpGpUpdateResult> EmployeeGatePassUpdating(EmpGpUpdate empGpUpdate)
        {
            if (empGpUpdate.SelectedEmpGpId <= 0)
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = false, Message = "Employee gatepass not found!", ErrorFieldId = "" });
       

            var connection = _dbConnection.GetConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                if (empGpUpdate.IsOut > 0)
                {
                    // Update for outgoing
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpEmpHeader]
                       SET [IsOutUpdated] = @IsOut
                          ,[IsInUpdate] = 0,
                           [GRUserId] = @GRUserId   
                     WHERE [ID] = @EmpGpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            IsOut = empGpUpdate.ActionType ? 1 : 2,
                            EmpGpId = empGpUpdate.SelectedEmpGpId,
                            GRUserId = _iSessionHelper.GetUserId()
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //EmpGpUpdateDetailList update details
                    if (empGpUpdate.EmpGpUpdateDetailList != null && empGpUpdate.EmpGpUpdateDetailList.Count > 0)
                    {
                        string sqlUpdateDetails = @"UPDATE [dbo].[TGPS_TrGpEmpDetails]
                            SET  [ActualOutTime] = @ActualTime
                            WHERE [Id] = @EmpGpDetailId";
                        foreach (var detail in empGpUpdate.EmpGpUpdateDetailList)
                        {
                            connection.Execute(
                                sqlUpdateDetails,
                                new 
                                {
                                    ActualTime = detail.TimeValue == ""? DateTime.Now.ToString() : DateTime.Today.ToString("yyyy-MM-dd") + " " + detail.TimeValue,
                                    EmpGpDetailId = detail.ID 
                                },


                                transaction: transaction,
                                commandType: CommandType.Text);
                        }
                    }

                }
                else
                {
                    // Update for receiving
                    string sqlUpdateM = @"UPDATE [dbo].[TGPS_TrGpEmpHeader]
                       SET [IsOutUpdated] = 1
                          ,[IsInUpdate] = @IsIn,
                           [GRUserId] = @GRUserId   
                     WHERE [ID] = @EmpGpId";

                    connection.Execute(
                        sqlUpdateM,
                        new
                        {
                            IsIn = empGpUpdate.ActionType ? 1 : 2,
                            EmpGpId = empGpUpdate.SelectedEmpGpId,
                            GRUserId = _iSessionHelper.GetUserId()
                        },
                        transaction: transaction,
                        commandType: CommandType.Text);

                    //EmpGpUpdateDetailList update details
                    if (empGpUpdate.EmpGpUpdateDetailList != null && empGpUpdate.EmpGpUpdateDetailList.Count > 0)
                    {
                        string sqlUpdateDetails = @"UPDATE [dbo].[TGPS_TrGpEmpDetails]
                            SET  [ActualInTime] = @ActualTime
                            WHERE [Id] = @EmpGpDetailId";
                        foreach (var detail in empGpUpdate.EmpGpUpdateDetailList)
                        {
                            connection.Execute(
                                sqlUpdateDetails,
                                new
                                {
                                    ActualTime = detail.TimeValue == "" ? DateTime.Now.ToString() : DateTime.Today.ToString("yyyy-MM-dd") + " " + detail.TimeValue,
                                    EmpGpDetailId = detail.ID
                                },


                                transaction: transaction,
                                commandType: CommandType.Text);
                        }
                    }

                }

                transaction.Commit();
                return await Task.FromResult(new EmpGpUpdateResult { IsSuccess = true, Message = "Employee Transaction completed successfully." });
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
