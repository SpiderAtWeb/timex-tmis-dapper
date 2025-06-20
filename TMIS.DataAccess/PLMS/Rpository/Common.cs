using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class Common(IDatabaseConnectionSys dbConnection, IUserControls userControls) : ICommon
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;

        public async Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync()
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResponseType, Customer, Seasons, SampleType, SampleStage, InquiryComment
                   FROM PLMS_VwInqListPending";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql);
        }

        public async Task<ModalShowVM> LoadModalDataAsync(string Id)
        {
            // Create the dynamic model
            var dynamicModel = new ModalShowVM
            {
                ArtWork = (await _dbConnection.GetConnection().QueryAsync<byte[]>(@"
                SELECT ImageSketch FROM PLMS_TrInqDetails WHERE (Id = @Id)",
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
                var activities = (await connection.QueryAsync<PLMSTrActivity>(@"
                    SELECT    TaskId, ActivityId, ActivityText, ActivityRequiredDate, ActivityComment, 
                              ActivityActualCmpltdDate, ActivityDoneComment, ActivityDoneBy, ActivityIsCompleted
                    FROM PLMS_VwTrActivityList WHERE (Id = @Id) ORDER BY TaskId",
                 new
                 {
                     Id
                 })).ToList();

                // Fetch sub-activities asynchronously
                var subActivities = (await connection.QueryAsync<PLMSTrActivity>(@"
                   SELECT     TaskId, ActivityId, SubActivityText, ActivityRequiredDate, ActivityComment, 
                              ActivityActualCmpltdDate, ActivityDoneComment, ActivityDoneBy, ActivityIsCompleted
                   FROM       PLMS_VwTrActivitySubList WHERE (Id = @Id) ORDER BY TaskId",
                 new
                 {
                     Id
                 })).ToList();

                // Group sub-activities by ActivityId
                var subActivityGroups = subActivities.GroupBy(sa => sa.ActivityId).ToDictionary(g => g.Key, g => g.ToList());
                int DueDates;
                DateTime today = DateTime.Today;

                foreach (var activity in activities)
                {
                    if (activity.ActivityIsCompleted)
                    {
                        // Calculate DueDates when the activity is completed
                        DueDates = (activity.ActivityRequiredDate != "" && activity.ActivityActualCmpltdDate != "")
                            ? (Convert.ToDateTime(activity.ActivityActualCmpltdDate) - Convert.ToDateTime(activity.ActivityRequiredDate!)).Days
                            : 0;
                    }
                    else
                    {
                        // Calculate DueDates when the activity is not completed and the required date is before today
                        DueDates = (activity.ActivityRequiredDate != "" && Convert.ToDateTime(activity.ActivityRequiredDate) < today)
                            ? (Convert.ToDateTime(activity.ActivityRequiredDate) - today).Days
                            : 0;
                    }


                    var activityModel = new PLMSTrActivity
                    {
                        TaskId = activity.TaskId,
                        ActivityText = activity.ActivityText,
                        ActivityRequiredDate = activity.ActivityRequiredDate != ""
                        ? Convert.ToDateTime(activity.ActivityRequiredDate).ToString("MM-dd-yy")
                        : "",
                        ActivityActualCmpltdDate = activity.ActivityActualCmpltdDate != ""
                           ? Convert.ToDateTime(activity.ActivityActualCmpltdDate).ToString("MM-dd-yy")
                           : "",
                        DueDates = DueDates,
                        ActivityComment = activity.ActivityComment,
                        ActivityDoneComment = activity.ActivityDoneComment,
                        ActivityDoneBy = activity.ActivityDoneBy,
                        ActivityIsCompleted = activity.ActivityIsCompleted,
                        SubActivityList = []
                    };

                    if (subActivityGroups.TryGetValue(activity.ActivityId, out List<PLMSTrActivity>? value))
                    {
                        foreach (var subActivity in value)
                        {

                            if (activity.ActivityIsCompleted)
                            {
                                // Calculate DueDates when the activity is completed
                                DueDates = (activity.ActivityRequiredDate != "" && subActivity.ActivityActualCmpltdDate != "")
                                    ? (Convert.ToDateTime(subActivity.ActivityActualCmpltdDate) - Convert.ToDateTime(subActivity.ActivityRequiredDate!)).Days
                                    : 0;
                            }
                            else
                            {
                                // Calculate DueDates when the activity is not completed and the required date is before today
                                DueDates = (activity.ActivityRequiredDate != "" && Convert.ToDateTime(subActivity.ActivityRequiredDate) < today)
                                    ? (Convert.ToDateTime(subActivity.ActivityRequiredDate) - today).Days
                                    : 0;
                            }

                            activityModel.SubActivityList.Add(new PLMSTrActivity
                            {
                                TaskId = subActivity.TaskId,
                                SubActivityText = subActivity.SubActivityText,
                                ActivityRequiredDate = subActivity.ActivityRequiredDate != ""
                                ? Convert.ToDateTime(subActivity.ActivityRequiredDate).ToString("MM-dd-yy")
                                : "",

                                ActivityActualCmpltdDate = subActivity.ActivityActualCmpltdDate != ""
                                   ? Convert.ToDateTime(subActivity.ActivityActualCmpltdDate).ToString("MM-dd-yy")
                                   : "",
                                DueDates = DueDates,
                                ActivityComment = subActivity.ActivityComment,
                                ActivityDoneComment = subActivity.ActivityDoneComment,
                                ActivityDoneBy = subActivity.ActivityDoneBy,
                                ActivityIsCompleted = subActivity.ActivityIsCompleted,
                            });
                        }
                    }
                    dynamicModel.ActivityList?.Add(activityModel);
                }
            }
            return dynamicModel;
        }

        public async Task<Models.PLMS.NewInquiryVM> LoadInquiryDropDowns()
        {
            var oInquiriesVM = new Models.PLMS.NewInquiryVM
            {
                InquiryTypesList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoInquiryTypes"),
                CustomersList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoCustomers"),
                SeasonsList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoSeasons"),
                SampleTypesList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoSampleTypes"),
                RoutingPresetsList = await LoadRouteDropAsync("PLMS_CPTemplateHeader"),
            };
            return oInquiriesVM;
        }

        private async Task<IEnumerable<SelectListItem>> LoadRouteDropAsync(string tableName)
        {
            string query = $@"SELECT CAST(Id AS NVARCHAR) AS Value, 
            CpName AS Text FROM {tableName} ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

    }
}
