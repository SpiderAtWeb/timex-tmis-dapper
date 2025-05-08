using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class NewInquiry(IDatabaseConnectionSys dbConnection, IPLMSLogdb pLMSLogdb) : INewInquiry
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        List<DateTime> companyDateList = [];

        public async Task<InquiryVM> LoadActsAndSubActsAsync(InquiryParams inqParas)
        {
            string checkIdQuery = @"SELECT Id
            FROM            PLMS_HpActivityScheduleHeader
            WHERE        (InquiryTypeId =@InquiryTypeId) AND 
            (ResponseTypeId = @ResponseTypeId) AND 
            (CustomerId = @CustomerId) AND 
            (SampleTypeId = @SampleTypeId) AND 
            (SampleStageId = @SampleStageId)";

            var parameters = new
            {
                inqParas.InquiryTypeId,
                inqParas.ResponseTypeId,
                inqParas.CustomerId,
                inqParas.SampleTypeId,
                inqParas.SampleStageId
            };

            int reacordId = 0;

            // Create the dynamic model
            var dynamicModel = new InquiryVM();

            using (var connection = _dbConnection.GetConnection())
            {
                reacordId = await connection.ExecuteScalarAsync<int>(checkIdQuery, parameters);
                if (reacordId == 0)
                {
                    return dynamicModel;
                }

                // Fetch activities asynchronously
                var activities = (await connection.QueryAsync<PLMSActivity>(@"
                    SELECT ActivityId, UserCategoryId, UserCategoryText, Days, ActivityText
                    FROM PLMS_VwActivityList
                    WHERE  (ActivityHeaderId = @ReacordId) ORDER BY Id",
                      new
                      {
                          ReacordId = reacordId
                      })).ToList();

                // Fetch sub-activities asynchronously
                // Fetch sub-activities asynchronously
                var subActivities = (await connection.QueryAsync<PLMSActivity>(@"
                    SELECT      ActivityId, SubActivityId, Days, UserCategoryId, UserCategoryText, SubActivityText
                    FROM            PLMS_VwActivitySubList
                    WHERE (ActivityHeaderId = @ReacordId) ORDER BY Id",
                 new
                 {
                     ReacordId = reacordId
                 })).ToList();

                // Group sub-activities by ActivityId
                var subActivityGroups = subActivities.GroupBy(sa => sa.ActivityId).ToDictionary(g => g.Key, g => g.ToList());
                int actDays = 0;

                int dateIndex = GetSelectedDateIndex(Convert.ToDateTime(inqParas.SelectedDate));

                foreach (var activity in activities)
                {
                    actDays += int.Parse(activity.Days);
                    var activityModel = new ActivityVM
                    {
                        ActivityId = activity.ActivityId,
                        ActiviytName = activity.ActivityText,
                        UserCategoryId = activity.UserCategoryId,
                        UserCategoryText = activity.UserCategoryText,
                        ActiviytValue = GetDateFromCalander(dateIndex + actDays),
                        SubActivityList = [] // Initialize the SubActivityList
                    };

                    if (subActivityGroups.TryGetValue(activity.ActivityId, out List<PLMSActivity>? value))
                    {
                        foreach (var subActivity in value)
                        {
                            actDays += int.Parse(subActivity.Days);
                            activityModel.SubActivityList.Add(new ActivityVM
                            {
                                ActivityId = subActivity.SubActivityId,
                                ActiviytName = subActivity.SubActivityText,
                                UserCategoryId = activity.UserCategoryId,
                                UserCategoryText = activity.UserCategoryText,
                                ActiviytValue = GetDateFromCalander(dateIndex + actDays)// Convert.ToDateTime(inqParas.SelectedDate).AddDays(actDays).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                    dynamicModel.ActivityList?.Add(activityModel);
                }
            }
            return dynamicModel;
        }

        private int GetSelectedDateIndex(DateTime selectedDate)
        {
            companyDateList = [];

            var query = "SELECT CalendarDate FROM MasterCompanyCalendar";

            using (var connection = _dbConnection.GetConnection())
            {
                companyDateList = connection.Query<DateTime>(query).ToList();
            }

            int selectedIndex = companyDateList.FindIndex(d => d == selectedDate);

            return selectedIndex;
        }

        private string GetDateFromCalander(int daysCount)
        {
            DateTime postionDate = companyDateList.ElementAtOrDefault(daysCount);
            return postionDate.ToString("yyyy-MM-dd");
        }

        public async Task<string> SaveMasterInquiryAsync(InquiryVM inquiryVM, IFormFile? artwork)
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

                    var inquiryHeaderId = await InsertInquiryHeaderAsync(connection, transaction, inquiryVM);
                    var inquiryDtId = await InsertInquiryDetailsAsync(connection, transaction, inquiryHeaderId, inquiryVM, artwork);
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

        private static async Task<int> InsertInquiryHeaderAsync(IDbConnection connection, IDbTransaction transaction, InquiryVM inquiryVM)
        {

            var inquiryRef = await connection.QuerySingleAsync<string>(
                "GenerateAndUpdateAutoNumber",
                commandType: CommandType.StoredProcedure,
                transaction: transaction);

            var sqlHeader = @"INSERT INTO [dbo].[PLMS_TrInqHeader]
               ([InquiryRef], [CustomerId], [InquiryTypeId], [StyleNo], 
                [StyleDesc], [InquiryRecDate], [IsCompleted])
               VALUES
               (@InquiryRef, @CustomerId, @InquiryTypeId, @StyleNo, 
                @StyleDesc, @InquiryRecDate, 0);
               SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.QuerySingleAsync<int>(sqlHeader, new
            {
                InquiryRef = inquiryRef,
                inquiryVM.Inquiry!.CustomerId,
                inquiryVM.Inquiry.InquiryTypeId,
                inquiryVM.Inquiry.StyleNo,
                inquiryVM.Inquiry.StyleDesc,
                inquiryVM.Inquiry.ColorCode,
                inquiryVM.Inquiry.InquiryRecDate,

            }, transaction);
        }

        private static async Task<int> InsertInquiryDetailsAsync(IDbConnection connection, IDbTransaction transaction, int inquiryHeaderId, InquiryVM inquiryVM, IFormFile? artwork)
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

            var sqlDetail = @"INSERT INTO [dbo].[PLMS_TrInqDetails]
             ([TrInqId], [CycleNo], [ResponseTypeId], [InquirySeasonId], [SampleTypeId], [SampleStageId], [ColorCode], [ImageSketch], [IsPriceUpdate],[IsSMVUpdate],
              [DateResponseReq], [InquiryComment], [IsApproved],[IsPriceStageAv], [IsSMVStageAv])
             VALUES
             (@TrInqId, 1, @ResponseTypeId, @InquirySeasonId,  @SampleTypeId, @SampleStageId, @ColorCode, @ImageSketch, 0, 0, @DateResponseReq, @InquiryComment, -1, @IsPriceStageAv, @IsSMVStageAv)
              SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.QuerySingleAsync<int>(sqlDetail, new
            {
                TrInqId = inquiryHeaderId,

                inquiryVM.Inquiry!.ResponseTypeId,
                inquiryVM.Inquiry.InquirySeasonId,
                inquiryVM.Inquiry.SampleTypeId,
                inquiryVM.Inquiry.SampleStageId,
                inquiryVM.Inquiry.ColorCode,
                ImageSketch = artworkBytes,
                DateResponseReq = inquiryVM.Inquiry?.InquiryReqDate,
                inquiryVM.Inquiry?.InquiryComment,
                inquiryVM.Inquiry?.IsPriceStageAv,
                inquiryVM.Inquiry?.IsSMVStageAv
            }, transaction);
        }

        private async Task InsertActivitiesAsync(IDbConnection connection, IDbTransaction transaction, int inquiryDtId, List<ActivityVM>? activityList)
        {
            foreach (var activity in activityList ?? [])
            {
                var activityId = await InsertActivityAsync(connection, transaction, inquiryDtId, activity);
                await InsertSubActivitiesAsync(connection, transaction, activityId, activity.SubActivityList);
            }
        }

        private static async Task<int> InsertActivityAsync(IDbConnection connection, IDbTransaction transaction, int inquiryDtId, ActivityVM activity)
        {
            var sqlActivity = @"INSERT INTO [dbo].[PLMS_TrInqDetailsActivity]
                       ([TrInqDtId], [UserCategoryId], [ActivityId], [ActivityRequiredDate], [ActivityComment], [ActivityIsCompleted])
                       VALUES
                       (@TrInqDtId, @UserCategoryId, @ActivityId, @ActivityRequiredDate, @ActivityComment, 0);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.QuerySingleAsync<int>(sqlActivity, new
            {
                TrInqDtId = inquiryDtId,
                activity.UserCategoryId,
                activity.ActivityId,
                ActivityRequiredDate = activity.ActiviytValue,
                activity.ActivityComment
            }, transaction);
        }

        private static async Task InsertSubActivitiesAsync(IDbConnection connection, IDbTransaction transaction, int activityId, List<ActivityVM>? subActivityList)
        {
            foreach (var subActivity in subActivityList ?? [])
            {
                var sqlSubActivity = @"INSERT INTO [dbo].[PLMS_TrInqDetailsActivitySub]
                               ([TrInqDtActId], [UserCategoryId], [SubActivityId], [SubActivityRequiredDate], [ActivityComment], [ActivityIsCompleted])
                               VALUES
                               (@TrInqDtActId,@UserCategoryId, @SubActivityId, @SubActivityRequiredDate, @ActivityComment, 0);";

                await connection.ExecuteAsync(sqlSubActivity, new
                {
                    TrInqDtActId = activityId,
                    subActivity.UserCategoryId,
                    SubActivityId = subActivity.ActivityId,
                    SubActivityRequiredDate = subActivity.ActiviytValue,
                    subActivity.ActivityComment
                }, transaction);
            }
        }
    }
}
