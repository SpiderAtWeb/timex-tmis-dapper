using Dapper;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
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

        private const long MaxZipSize = 5 * 1024 * 1024;

        List<DateTime> companyDateList = [];
        public async Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesUserIdAsync()
        {
            string sql = @"SELECT DISTINCT Id, CONCAT(InquiryRef, '.v', [CycleNo]) AS InquiryRef, [StyleNo], [StyleDesc], [ColorCode], 
                          [InquiryType], [Customer], [SeasonName], [SampleType], [ArtWork], [Remarks]
                   FROM PLMS_VwInquiryListPendingUserId WHERE [UserId] = @Id";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql, new { Id = _iSessionHelper.GetUserId() });
        }

        public async Task<string> SaveTasksAndSubTasksAsync(SaveTasks saveTasks)
        {
            using (var connection = _dbConnection.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Get InquiryId
                    string inquiryIdQuery = @"SELECT TrInqDtId FROM [dbo].[PLMS_TrInquiryActivityDetails] WHERE Id = @Id";

                    int inquiryId = 0;

                    if (saveTasks.MainTasks == null || saveTasks.MainTasks.Count == 0)
                    {
                        inquiryIdQuery = @"SELECT PLMS_TrInquiryActivityDetails.TrInqDtId
                                   FROM PLMS_TrInquiryActivityDetailsSub
                                   INNER JOIN PLMS_TrInquiryActivityDetails ON PLMS_TrInquiryActivityDetailsSub.TrInqDtActId = PLMS_TrInquiryActivityDetails.Id
                                   WHERE PLMS_TrInquiryActivityDetailsSub.Id = @Id";

                        inquiryId = await connection.QuerySingleAsync<int>(
                            inquiryIdQuery,
                            new { Id = saveTasks.SubTasks.First().TaskId },
                            transaction
                        );
                    }
                    else
                    {
                        inquiryId = await connection.QuerySingleAsync<int>(
                            inquiryIdQuery,
                            new { Id = saveTasks.MainTasks.First().TaskId },
                            transaction
                        );
                    }

                    string uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "taskfiles");
                    if (!Directory.Exists(uploadRoot))
                        Directory.CreateDirectory(uploadRoot);

                    // 🟩 Handle Main Tasks
                    foreach (var activity in saveTasks.MainTasks!)
                    {
                        string? filePath = null;

                        if (activity.ZipFile != null && activity.ZipFile.Length > 0)
                        {
                            if (activity.ZipFile.Length > MaxZipSize)
                            {
                                return $"ZIP file for task exceeds the 5MB size limit.";
                            }

                            string fileName = $"main_{activity.TaskId}_{Path.GetFileName(activity.ZipFile.FileName)}";
                            filePath = Path.Combine("uploads/taskfiles", fileName); // Relative path
                            string fullPath = Path.Combine("wwwroot", filePath);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await activity.ZipFile.CopyToAsync(stream);
                            }
                        }

                        // Update task with optional file path
                        string mainTaskQuery = @"
                        UPDATE [dbo].[PLMS_TrInquiryActivityDetails]
                        SET [IsCompleted] = 1,
                            [ActualCompletedDate] = GETDATE(),
                            [Remarks] = @Comment,
                            [DoneBy] = @User,
                            ZipFilePath = @FilePath
                        WHERE Id = @Id";

                        await connection.ExecuteAsync(mainTaskQuery, new
                        {
                            Id = activity.TaskId,
                            activity.Comment,
                            FilePath = filePath,
                            User = _iSessionHelper.GetUserId()
                        }, transaction);

                        await UpdateTaskLogAsync(connection, transaction, inquiryId, activity.TaskId, true);

                        await UpdateXdays((activity.TaskId + 1), inquiryId, connection, transaction);
                    }

                    // 🟩 Handle Sub Tasks
                    foreach (var activity in saveTasks.SubTasks)
                    {
                        string? filePath = null;

                        if (activity.ZipFile != null && activity.ZipFile.Length > 0)
                        {
                            if (activity.ZipFile.Length > MaxZipSize)
                            {
                                return $"ZIP file for task exceeds the 5MB size limit.";
                            }

                            string fileName = $"sub_{activity.TaskId}_{Path.GetFileName(activity.ZipFile.FileName)}";
                            filePath = Path.Combine("uploads/taskfiles", fileName); // Relative
                            string fullPath = Path.Combine("wwwroot", filePath);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await activity.ZipFile.CopyToAsync(stream);
                            }
                        }

                        string subTaskQuery = @"
                        UPDATE [dbo].[PLMS_TrInquiryActivityDetailsSub]
                        SET IsCompleted = 1,
                            ActualCompletedDate = GETDATE(),
                            Remarks = @Comment,
                            DoneBy = @User,
                            ZipFilePath = @FilePath
                        WHERE Id = @Id";

                        await connection.ExecuteAsync(subTaskQuery, new
                        {
                            Id = activity.TaskId,
                            activity.Comment,
                            FilePath = filePath,
                            User = _iSessionHelper.GetUserId()
                        }, transaction);

                        await UpdateTaskLogAsync(connection, transaction, inquiryId, activity.TaskId, false);
                    }

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

        private async Task<int> GetSelectedDateIndexAsync(DateTime selectedDate, IDbConnection connection, IDbTransaction transaction)
        {
            companyDateList = (await connection.QueryAsync<DateTime>(
                "SELECT CalendarDate FROM COMN_MasterCompCalendar",
                transaction: transaction  // ✅ Important
            )).ToList();

            int selectedIndex = companyDateList.FindIndex(d => d == selectedDate);

            if (selectedIndex == -1)
            {
                selectedIndex = companyDateList.FindIndex(d => d > selectedDate);
            }

            return selectedIndex;
        }

        private string GetDateFromCalander(int daysCount)
        {
            DateTime postionDate = companyDateList.ElementAtOrDefault(daysCount);
            return postionDate.ToString("yyyy-MM-dd");
        }

        private async Task UpdateXdays(int inqId, int trINQDTId, IDbConnection connection, IDbTransaction transaction)
        {
            var sqlCheck = @"SELECT IsAwaitingTask FROM PLMS_TrInquiryActivityDetails WHERE Id = @Id";
            var isAwait = await connection.QuerySingleAsync<bool>(sqlCheck, new { Id = inqId }, transaction);

            if (!isAwait) return;

            var parameters = new { TrINQDTId = trINQDTId, Id = inqId };
            var result = (await connection.QueryAsync<OffsetDayResult>(
                "GetOffsetDaysRange",
                param: parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure
            )).ToList();

            if (result.Count == 0) return;

            // Fetch startDateIndex first, do not call another query inside loop
            int startDateIndex = await GetSelectedDateIndexAsync(DateTime.Now, connection, transaction) - 1;

            foreach (var res in result)
            {
                string activityDate = GetDateFromCalander(startDateIndex + res.OffsetDays);

                var updateQuery = @"
                UPDATE PLMS_TrInquiryActivityDetails
                SET RequiredDate = @RequiredDate
                WHERE TrINQDTId = @TrINQDTId AND Id = @Id";

                await connection.ExecuteAsync(updateQuery, new { RequiredDate = activityDate, TrINQDTId = trINQDTId, Id = res.Id }, transaction);
            }
        }

        public async Task<ModalShowVM> LoadModalDataUserIdAsync(string Id)
        {
            // Create the dynamic model
            var dynamicModel = new ModalShowVM
            {
                ArtWork = (await _dbConnection.GetConnection().QueryAsync<byte[]>(@"
                SELECT ArtWork FROM PLMS_TrInquiryDetails WHERE (Id = @Id)",
                new
                {
                    Id
                })).FirstOrDefault(),

                LogStrings = (await _dbConnection.GetConnection().QueryAsync<string>(@"
                SELECT CONCAT(TrDateTime, ' - ', TrLog, ' - ', TrUser) 
                FROM PLMS_TrLogger 
                WHERE InqId = @InqId 
                ORDER BY TrDateTime DESC 
                OFFSET 0 ROWS FETCH NEXT 200 ROWS ONLY",
                new { InqId = Id }
            )).ToArray()

            };

            using (var connection = _dbConnection.GetConnection())
            {
                // Fetch activities asynchronously
                var activities = (await connection.QueryAsync<PLMSTrActivity>(@"SELECT Id, ActivityId, ActivityName, RequiredDate, PlanRemakrs, 
                ActualCompletedDate, Remarks, DoneBy, IsCompleted, ZipFilePath  FROM PLMS_VwTrActivityListUserId WHERE (TrINQDTId = @Id) AND (UserId = @UserId) ORDER BY Id",
                 new
                 {
                     Id,
                     UserId = _iSessionHelper.GetUserId()
                 })).ToList();

                // Fetch sub-activities asynchronously
                var subActivities = (await connection.QueryAsync<PLMSTrActivity>(@"
                SELECT  Id, ActivityId, ActivityName, RequiredDate, PlanRemakrs, ActualCompletedDate, Remarks, DoneBy, IsCompleted, ZipFilePath
                FROM       PLMS_VwTrActivitySubListUserId WHERE (TrINQDTId = @Id) AND (UserId = @UserId) ORDER BY Id",
                 new
                 {
                     Id,
                     UserId = _iSessionHelper.GetUserId()
                 })).ToList();

                // Group sub-activities by ActivityId
                var subActivityGroups = subActivities.GroupBy(sa => sa.ActivityId).ToDictionary(g => g.Key, g => g.ToList());
                int DueDates;
                DateTime today = DateTime.Today;

                foreach (var activity in activities)
                {
                    if (activity.IsCompleted)
                    {
                        // Calculate DueDates when the activity is completed
                        DueDates = (activity.RequiredDate != "" && activity.ActualCompletedDate != "")
                            ? (Convert.ToDateTime(activity.ActualCompletedDate) - Convert.ToDateTime(activity.RequiredDate!)).Days
                            : 0;
                    }
                    else
                    {
                        // Calculate DueDates when the activity is not completed and the required date is before today
                        DueDates = (activity.RequiredDate != "" && Convert.ToDateTime(activity.RequiredDate) < today)
                            ? (Convert.ToDateTime(activity.RequiredDate) - today).Days
                            : 0;
                    }


                    var activityModel = new PLMSTrActivity
                    {
                        Id = activity.Id,
                        ActivityName = activity.ActivityName,
                        RequiredDate = activity.RequiredDate != ""
                        ? Convert.ToDateTime(activity.RequiredDate).ToString("MM-dd-yy")
                        : "",
                        ActualCompletedDate = activity.ActualCompletedDate != ""
                           ? Convert.ToDateTime(activity.ActualCompletedDate).ToString("MM-dd-yy")
                           : "",
                        DueDates = DueDates,
                        PlanRemakrs = activity.PlanRemakrs,
                        Remarks = activity.Remarks,
                        DoneBy = activity.DoneBy,
                        IsCompleted = activity.IsCompleted,
                        ZipFilePath = activity.ZipFilePath,
                        SubActivityList = []
                    };

                    if (subActivityGroups.TryGetValue(activity.ActivityId, out List<PLMSTrActivity>? value))
                    {
                        foreach (var subActivity in value)
                        {

                            if (activity.IsCompleted)
                            {
                                // Calculate DueDates when the activity is completed
                                DueDates = (activity.RequiredDate != "" && subActivity.ActualCompletedDate != "")
                                    ? (Convert.ToDateTime(subActivity.ActualCompletedDate) - Convert.ToDateTime(subActivity.RequiredDate!)).Days
                                    : 0;
                            }
                            else
                            {
                                // Calculate DueDates when the activity is not completed and the required date is before today
                                DueDates = (activity.RequiredDate != "" && Convert.ToDateTime(subActivity.RequiredDate) < today)
                                    ? (Convert.ToDateTime(subActivity.RequiredDate) - today).Days
                                    : 0;
                            }

                            activityModel.SubActivityList.Add(new PLMSTrActivity
                            {
                                Id = subActivity.Id,
                                ActivityName = subActivity.ActivityName,
                                RequiredDate = subActivity.RequiredDate != ""
                                ? Convert.ToDateTime(subActivity.RequiredDate).ToString("MM-dd-yy")
                                : "",

                                ActualCompletedDate = subActivity.ActualCompletedDate != ""
                                   ? Convert.ToDateTime(subActivity.ActualCompletedDate).ToString("MM-dd-yy")
                                   : "",
                                DueDates = DueDates,
                                PlanRemakrs = subActivity.PlanRemakrs,
                                Remarks = subActivity.Remarks,
                                DoneBy = subActivity.DoneBy,
                                IsCompleted = subActivity.IsCompleted,
                                ZipFilePath = subActivity.ZipFilePath
                            });
                        }
                    }
                    dynamicModel.ActivityList?.Add(activityModel);
                }
            }
            return dynamicModel;
        }

        private async Task UpdateTaskLogAsync(IDbConnection connection, IDbTransaction transaction, int inquiryId, int taskId, bool isMain)
        {
            var taskNameQuery = @"
            SELECT   PLMS_MasterTwoActivityTypes.PropName
            FROM PLMS_TrInquiryActivityDetails INNER JOIN
            PLMS_MasterTwoActivityTypes ON PLMS_TrInquiryActivityDetails.ActivityId = PLMS_MasterTwoActivityTypes.Id
            WHERE (PLMS_TrInquiryActivityDetails.Id = @Id)";

            //Get Taskname
            if (!isMain)
            {
                taskNameQuery = @"
                  SELECT        dbo.PLMS_MasterTwoActivityTypes.PropName
                 FROM            dbo.PLMS_TrInquiryActivityDetailsSub INNER JOIN
                 dbo.PLMS_MasterTwoActivityTypes ON dbo.PLMS_TrInquiryActivityDetailsSub.Id = dbo.PLMS_MasterTwoActivityTypes.Id
                 WHERE        (dbo.PLMS_TrInquiryActivityDetailsSub.Id = @Id)";
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


