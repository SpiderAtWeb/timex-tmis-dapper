using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class NewInquiry(IDatabaseConnectionSys dbConnection, IPLMSLogdb pLMSLogdb, IUserControls userControls) : INewInquiry
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;
        private readonly IUserControls _userControls = userControls;

       List<DateTime> companyDateList = [];

        public async Task<NewInquiryVM> LoadActsAndSubActsAsync(RoutePresetParas paras)
        {
            var model = new NewInquiryVM();

            using var connection = _dbConnection.GetConnection();

            var activities = (await connection.QueryAsync<PLMSActivity>(@"
            SELECT ActivityId, UserCategoryId, UserCategoryText, Days, ActivityText, IsAwaitTask
            FROM PLMS_VwActivityList
            WHERE CpHeaderId = @ReacordId
            ORDER BY Id", new { ReacordId = paras.PresetId })).ToList();

            var subActivities = (await connection.QueryAsync<PLMSActivity>(@"
            SELECT ActivityId, SubActivityId, Days, UserCategoryId, UserCategoryText, SubActivityText, IsAwaitTask
            FROM PLMS_VwActivitySubList
            WHERE CpHeaderId = @ReacordId
            ORDER BY Id", new { ReacordId = paras.PresetId })).ToList();

            var subActivityGroups = subActivities
                .GroupBy(sa => sa.ActivityId)
                .ToDictionary(g => g.Key, g => g.ToList());

            int dateIndex = await  GetSelectedDateIndex(Convert.ToDateTime(paras.SelectedDate), connection) - 1;
            int activityRunningDays = 0;
            bool isAfterActivityAwait = false;

            foreach (var activity in activities)
            {
                int currentDays = int.Parse(activity.Days);

                if (activity.IsAwaitTask)
                {
                    activityRunningDays = currentDays;
                    isAfterActivityAwait = true;
                }
                else
                {
                    activityRunningDays += currentDays;
                }

                string activityDate = activity.IsAwaitTask || isAfterActivityAwait
                    ? $"(X+{activityRunningDays})"
                    : GetDateFromCalander(dateIndex + activityRunningDays);

                var activityVM = new ActivityVM
                {
                    ActivityId = activity.ActivityId,
                    ActiviytName = activity.ActivityText + (activity.IsAwaitTask ? " <span class='badge rounded-pill bg-label-danger' style='color: red;'>Awaiting-Task</span>" : ""),
                    IsAwaitingTask = activity.IsAwaitTask,
                    UserCategoryId = activity.UserCategoryId,
                    UserCategoryText = activity.UserCategoryText,
                    ActiviytValue = activityDate,
                    SubActivityList = []
                };

                if (subActivityGroups.TryGetValue(activity.ActivityId, out var subList))
                {
                    int subRunningDays = activityRunningDays;
                    bool isAfterSubAwait = false;

                    foreach (var sub in subList)
                    {
                        int subDays = int.Parse(sub.Days);

                        if (sub.IsAwaitTask)
                        {
                            subRunningDays = subDays;
                            isAfterSubAwait = true;
                        }
                        else
                        {
                            subRunningDays += subDays;
                        }

                        string subDate = sub.IsAwaitTask || isAfterSubAwait
                            ? $"(X+{subRunningDays})"
                            : GetDateFromCalander(dateIndex + subRunningDays);

                        activityVM.SubActivityList.Add(new ActivityVM
                        {
                            ActivityId = sub.SubActivityId,
                            ActiviytName = sub.SubActivityText,
                            IsAwaitingTask = sub.IsAwaitTask,
                            UserCategoryId = sub.UserCategoryId,
                            UserCategoryText = sub.UserCategoryText,
                            ActiviytValue = subDate
                        });
                    }
                }

                model.ActivityList?.Add(activityVM);
            }

            return model;
        }

        public async Task<int> GetSelectedDateIndex(DateTime selectedDate, IDbConnection connection)
        {

            companyDateList = [];

            var query = "SELECT CalendarDate FROM COMN_MasterCompCalendar";

            companyDateList = [.. await connection.QueryAsync<DateTime>(query)];            

            int selectedIndex = companyDateList.FindIndex(d => d == selectedDate);

            if (selectedIndex == -1)
            {
                selectedIndex = companyDateList.FindIndex(d => d > selectedDate);
            }

            return selectedIndex;
        }

        public string GetDateFromCalander(int daysCount)
        {
            DateTime postionDate = companyDateList.ElementAtOrDefault(daysCount);
            return postionDate.ToString("yyyy-MM-dd");
        }

        public async Task<string> SaveInquiryAsync(NewInquiryVM inquiryVM, IFormFile? artwork)
        {
            var connection = _dbConnection.GetConnection();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    if (inquiryVM == null)
                    {
                        throw new ArgumentNullException(nameof(inquiryVM), "InquiryVM cannot be null.");
                    }

                    var inquiryHeaderId = await InsertHeaderAsync(connection, transaction, inquiryVM);
                    var inquiryDtId = await InsertDetailsAsync(connection, transaction, inquiryHeaderId, inquiryVM, artwork);

                    await InsertSizeQtysAsync(connection, transaction, inquiryDtId, inquiryVM.SizeQtysList);
                    await InsertActivitiesAsync(connection, transaction, inquiryDtId, inquiryVM.ActivityList);

                    //Log
                    Logdb logdb = new()
                    {
                        TrObjectId = inquiryDtId,
                        TrLog = "NEW INQUIRY CREATED"
                    };

                    await _pLMSLogdb.InsertLogTrans(connection, transaction, logdb);

                    // Commit the transaction if all operations are successful
                    transaction.Commit();

                    return "Success: Inquiry saved successfully.";
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    return $"Error: {ex.Message}";
                }
            }
        }

        private async Task<int> InsertHeaderAsync(IDbConnection connection, IDbTransaction transaction, NewInquiryVM inquiryVM)
        {
            string referenceNumber = await _userControls.GenerateRefAsync(connection, transaction, "[PLMS_XysGenerateNumber]", "PLM");

            var sqlHeader = @"INSERT INTO [dbo].[PLMS_TrInquiryHeader]
               ([InquiryRef]
               ,[CustomerId]
               ,[InquiryTypeId]
               ,[StyleNo]
               ,[StyleDesc]
               ,[IsCompleted])
                VALUES
               (@InquiryRef
               ,@CustomerId
               ,@InquiryTypeId
               ,@StyleNo
               ,@StyleDesc
               ,0);
               SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.QuerySingleAsync<int>(sqlHeader, new
            {
                InquiryRef = referenceNumber,
                inquiryVM.CustomerId,
                inquiryVM.InquiryTypeId,
                inquiryVM.StyleNo,
                inquiryVM.StyleDesc
            }, transaction);
        }

        public static async Task<int> InsertDetailsAsync(IDbConnection connection, IDbTransaction transaction, int inquiryHeaderId, NewInquiryVM inquiryVM, IFormFile? artwork)
        {
            byte[]? artworkBytes = null;

            if (artwork != null && artwork.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await artwork.CopyToAsync(memoryStream);
                    artworkBytes = memoryStream.ToArray();
                }
            }

            var sqlDetail = @"INSERT INTO [dbo].[PLMS_TrInquiryDetails]
                           ([TrINQId]
                           ,[CycleNo]
                           ,[ColorCode]
                           ,[Season]
                           ,[SampleTypeId]
                           ,[IsPriceStageAv]
                           ,[IsSMVStageAv]
                           ,[ArtWork]
                           ,[DateReceived]
                           ,[Remarks])
                     VALUES
                           (@TrINQId
                           ,@cycleNo
                           ,@ColorCode
                           ,@Season
                           ,@SampleTypeId
                           ,@IsPriceStageAv
                           ,@IsSMVStageAv
                           ,@ArtWork
                           ,@DateReceived
                           ,@Remarks);
              SELECT CAST(SCOPE_IDENTITY() AS INT);";


            var query = @"SELECT ISNULL(MAX(CycleNo), 0) + 1 FROM PLMS_TrInquiryDetails WHERE TrINQId = @InquiryId";
            var cycleNo = await connection.QuerySingleAsync<int>(query, new { InquiryId = inquiryHeaderId, }, transaction);

            return await connection.QuerySingleAsync<int>(sqlDetail, new
            {
                TrINQId = inquiryHeaderId,
                cycleNo,
                inquiryVM.ColorCode,
                inquiryVM.Season,
                inquiryVM.SampleTypeId,
                inquiryVM.IsPriceStageAv,
                inquiryVM.IsSMVStageAv,
                ArtWork = artworkBytes,
                DateReceived = inquiryVM.ReceivedDate,
                inquiryVM.Remarks,
            }, transaction);
        }

        public static async Task InsertActivitiesAsync(IDbConnection connection, IDbTransaction transaction, int inquiryDtId, List<ActivityVM>? activityList)
        {
            foreach (var activity in activityList ?? [])
            {
                var activityId = await InsertActivityAsync(connection, transaction, inquiryDtId, activity);
                await InsertSubActivitiesAsync(connection, transaction, activityId, activity.SubActivityList);
            }
        }

        private async Task InsertSizeQtysAsync(IDbConnection connection, IDbTransaction transaction, int inquiryDtId, List<SizeQtys>? sizeQtys)
        {
            foreach (var oSizeQtys in sizeQtys ?? [])
            {
                var sqlActivity = @"INSERT INTO [dbo].[PLMS_TrInquirySizeQtys]
                                       ([TrINQDTId]
                                       ,[SizeName]
                                       ,[SizeQty])
                                 VALUES
                                       (@TrInqDtId
                                       ,@SizeName
                                       ,@SizeQty)";

                await connection.ExecuteAsync(sqlActivity, new
                {
                    TrInqDtId = inquiryDtId,
                    oSizeQtys.SizeName,
                    oSizeQtys.SizeQty
                }, transaction);
            }
        }

        private static async Task<int> InsertActivityAsync(IDbConnection connection, IDbTransaction transaction, int inquiryDtId, ActivityVM activity)
        {
            var sqlActivity = @"INSERT INTO [dbo].[PLMS_TrInquiryActivityDetails]
                               ([TrINQDTId]
                               ,[UserCategoryId]
                               ,[ActivityId]
                               ,[IsAwaitingTask]
                               ,[ReqDateString]
                               ,[RequiredDate]
                               ,[PlanRemakrs]
                               ,[IsCompleted])
                         VALUES
                               (@TrInqDtId
                               ,@UserCategoryId
                               ,@ActivityId
                               ,@IsAwaitingTask
                               ,@ReqDateString
                               ,@RequiredDate
                               ,@PlanRemakrs
                               ,0);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.QuerySingleAsync<int>(sqlActivity, new
            {
                TrInqDtId = inquiryDtId,
                activity.UserCategoryId,
                activity.ActivityId,
                activity.IsAwaitingTask,
                ReqDateString = activity.ActiviytValue,
                RequiredDate = TryParseDate(activity.ActiviytValue),
                PlanRemakrs = activity.ActivityComment
            }, transaction);
        }

        private static DateTime? TryParseDate(string value)
        {
            return DateTime.TryParse(value, out var parsedDate) ? parsedDate : null;
        }

        private static async Task InsertSubActivitiesAsync(IDbConnection connection, IDbTransaction transaction, int activityId, List<ActivityVM>? subActivityList)
        {
            foreach (var subActivity in subActivityList ?? [])
            {
                var sqlActivity = @"INSERT INTO [dbo].[PLMS_TrInquiryActivityDetails]
                               ([TrINQDTActId]
                               ,[UserCategoryId]
                               ,[ActivityId]
                               ,[IsAwaitingTask]
                               ,[ReqDateString]
                               ,[RequiredDate]
                               ,[PlanRemakrs]
                               ,[IsCompleted])
                         VALUES
                               (@TrINQDTActId
                               ,@UserCategoryId
                               ,@ActivityId
                               ,@IsAwaitingTask
                               ,@ReqDateString
                               ,@RequiredDate
                               ,@PlanRemakrs
                               ,0)";

                await connection.QuerySingleAsync(sqlActivity, new
                {
                    TrINQDTActId = activityId,
                    subActivity.UserCategoryId,
                    subActivity.ActivityId,
                    subActivity.IsAwaitingTask,
                    ReqDateString = subActivity.ActiviytValue,
                    RequiredDate = TryParseDate(subActivity.ActiviytValue),
                    PlanRemakrs = subActivity.ActivityComment
                }, transaction);
            }
        }
    }
}
