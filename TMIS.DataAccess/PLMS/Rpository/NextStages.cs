using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class NextStages(IDatabaseConnectionSys dbConnection, IUserControls userControls, IPLMSLogdb pLMSLogdb) : INextStages
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<NextStageInquiryVM> LoadNextInquiryDropDowns(string id)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                string query = @"SELECT 
                            [Id], [CustomerId], [InquiryTypeId], 
                            CustomerName, InquiryType, StyleNo, StyleDesc, ColorCode 
                         FROM  
                            PLMS_VwInqHeader
                         WHERE     
                            DtId = @InquiryId";

                // Await the asynchronous database query to fetch the Inquiry
                var inquiry = await connection.QueryFirstOrDefaultAsync<Inquiry>(query, new { InquiryId = id });

                // Create and populate the NextStageInquiryVM
                var oNextStageInquiryVM = new NextStageInquiryVM
                {
                    InquiryTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdInquiryTypes"),
                    ResponseTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdReponseTypes"),
                    SeasonsList = await _userControls.LoadDropDownsAsync("PLMS_MdInquirySeason"),
                    SampleTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdExtendSub"),
                    SampleStagesList = await _userControls.LoadDropDownsAsync("PLMS_MdExtend"),
                    Inquiry = inquiry
                };

                return oNextStageInquiryVM;
            }
        }

        public async Task<string> SaveNextInquiryAsync(InquiryVM inquiryVM, IFormFile? artwork)
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

                    var inquiryDtId = await InsertInquiryDetailsAsync(connection, transaction, inquiryVM, artwork);
                    await InsertActivitiesAsync(connection, transaction, inquiryDtId, inquiryVM.ActivityList);

                    Logdb logdb = new()
                    {
                        TrObjectId = inquiryDtId,
                        TrLog = "NEW INQUIRY STAGE CREATED"
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

        private static async Task<int> InsertInquiryDetailsAsync(IDbConnection connection, IDbTransaction transaction, InquiryVM inquiryVM, IFormFile? artwork)
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

            var query = @"SELECT ISNULL(MAX(CycleNo), 0) + 1 FROM PLMS_TrInqDetails WHERE TrInqId = @InquiryId";
            var cycleNo = await connection.QuerySingleAsync<int>(query, new { InquiryId = inquiryVM.Inquiry!.Id, }, transaction);

            var sqlDetail = @"INSERT INTO [dbo].[PLMS_TrInqDetails]
             ([TrInqId], [CycleNo], [ResponseTypeId], [InquirySeasonId], [SampleTypeId], [SampleStageId], [ColorCode], [ImageSketch],
              [DateResponseReq], [IsPriceUpdate], [InquiryComment], [IsApproved])
             VALUES
             (@TrInqId, @CycleNo, @ResponseTypeId, @InquirySeasonId,  @SampleTypeId, @SampleStageId, @ColorCode, @ImageSketch, @DateResponseReq, 0, @InquiryComment, -1)
              SELECT CAST(SCOPE_IDENTITY() AS INT);";



            return await connection.QuerySingleAsync<int>(sqlDetail, new
            {
                TrInqId = inquiryVM.Inquiry!.Id,
                CycleNo = cycleNo,
                inquiryVM.Inquiry!.ResponseTypeId,
                inquiryVM.Inquiry.InquirySeasonId,
                inquiryVM.Inquiry.SampleTypeId,
                inquiryVM.Inquiry.SampleStageId,
                inquiryVM.Inquiry.ColorCode,
                ImageSketch = artworkBytes,
                DateResponseReq = inquiryVM.Inquiry?.InquiryReqDate,
                inquiryVM.Inquiry?.InquiryComment
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
