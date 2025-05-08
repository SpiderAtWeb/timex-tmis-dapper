using Dapper;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class TaskCompletion(IDatabaseConnectionSys dbConnection, IUserControls userControls, ISessionHelper sessionHelper, IPLMSLogdb pLMSLogdb) : ITaskCompletion
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<IEnumerable<ShowInquiryDataVM>> GetTasksListAsync()
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResposeType, Customer, Seasons, SampleType, SampleStage, InquiryComment
                   FROM PLMS_VwPendActivityList";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql);
        }

        public async Task<string> SaveTasksAndSubTasksAsync(SaveTasks saveTasks)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //Get InquiryId
                        var inquiryIdQuery = @"
                            SELECT TrInqDtId
                            FROM [dbo].[PLMS_TrInqDetailsActivity]
                            WHERE Id = @Id";
                        var inquiryId = 0;

                        if (saveTasks.MainTasks == null || saveTasks.MainTasks.Count == 0)
                        {
                            inquiryIdQuery = @"SELECT PLMS_TrInqDetailsActivity.TrInqDtId
                            FROM PLMS_TrInqDetailsActivitySub INNER JOIN
                            PLMS_TrInqDetailsActivity ON PLMS_TrInqDetailsActivitySub.TrInqDtActId = PLMS_TrInqDetailsActivity.Id
                            WHERE (PLMS_TrInqDetailsActivitySub.Id = @Id)";

                            inquiryId = await connection.QuerySingleAsync<int>(inquiryIdQuery, new { Id = saveTasks.SubTasks.First().TaskId }, transaction);
                        }
                        else
                        {
                            inquiryId = await connection.QuerySingleAsync<int>(inquiryIdQuery, new { Id = saveTasks.MainTasks.First().TaskId }, transaction);
                        }


                        foreach (var activity in saveTasks.MainTasks!)
                        {
                            // Update the main task
                            var mainTaskQuery = @"
                            UPDATE [dbo].[PLMS_TrInqDetailsActivity]
                            SET [ActivityIsCompleted] = 1,
                                [ActivityActualCmpltdDate] = GETDATE(),
                                [ActivityDoneComment] = @Comment,
                                [ActivityDoneBy] = @User
                            WHERE Id = @Id";

                            await connection.ExecuteAsync(mainTaskQuery, new
                            {
                                Id = activity.TaskId,
                                activity.Comment,
                                User = _iSessionHelper.GetUserName().ToUpper() // Replace with actual username
                            }, transaction);

                            await UpdateTaskLogAsync(connection, transaction, inquiryId, activity.TaskId, true);
                        }

                        foreach (var activity in saveTasks.SubTasks)
                        {
                            // Update the main task
                            var mainTaskQuery = @"
                            UPDATE [dbo].[PLMS_TrInqDetailsActivitySub]
                            SET [ActivityIsCompleted] = 1,
                                [ActivityActualCmpltdDate] = GETDATE(),
                                [ActivityDoneComment] = @Comment,
                                [ActivityDoneBy] = @User
                            WHERE Id = @Id";

                            await connection.ExecuteAsync(mainTaskQuery, new
                            {
                                Id = activity.TaskId,
                                activity.Comment,
                                User = _iSessionHelper.GetUserName().ToUpper() // Replace with actual username
                            }, transaction);

                            await UpdateTaskLogAsync(connection, transaction, inquiryId, activity.TaskId, false);
                        }

                        // Commit the transaction
                        transaction.Commit();

                        return "Tasks saved successfully.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error saving tasks: " + ex.Message);
                    }
                }
            }
        }

        private async Task UpdateTaskLogAsync(IDbConnection connection, IDbTransaction transaction, int inquiryId, int taskId, bool isMain)
        {
            var taskNameQuery = @"
            SELECT   PLMS_MdActivityTypes.PropName
            FROM PLMS_TrInqDetailsActivity INNER JOIN
            PLMS_MdActivityTypes ON PLMS_TrInqDetailsActivity.ActivityId = PLMS_MdActivityTypes.Id
            WHERE (PLMS_TrInqDetailsActivity.Id = @Id)";

            //Get Taskname
            if (!isMain)
            {
                taskNameQuery = @"
               SELECT        dbo.PLMS_MdActivityTypes.PropName
               FROM            dbo.PLMS_TrInqDetailsActivitySub INNER JOIN
               dbo.PLMS_MdActivityTypes ON dbo.PLMS_TrInqDetailsActivitySub.SubActivityId = dbo.PLMS_MdActivityTypes.Id
               WHERE        (dbo.PLMS_TrInqDetailsActivitySub.Id = @Id)";
            }

            var taskName = await connection.QuerySingleAsync<string>(taskNameQuery, new { Id = taskId }, transaction);

            //Log
            Logdb logdb = new()
            {
                TrObjectId = inquiryId,
                TrLog = taskName + " TASK UPDATED"
            };

            await _pLMSLogdb.InsertLogTrans(connection, transaction, logdb);
        }
    }
}


