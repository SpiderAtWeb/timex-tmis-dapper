using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class PLMSCommon(IDatabaseConnectionSys dbConnection, IUserControls userControls, ISessionHelper sessionHelper) : IPLMSCommon
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync()
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', [CycleNo]) AS InquiryRef, [StyleNo], [StyleDesc], [ColorCode], 
                          [InquiryType], [Customer], [SeasonName], [SampleType], [ArtWork], [Remarks]
                   FROM PLMS_VwInquiryListPending";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql);
        }
             

        public async Task<ModalShowVM> LoadModalDataAsync(string Id)
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
                ActualCompletedDate, Remarks, DoneBy, IsCompleted, ZipFilePath  FROM PLMS_VwTrActivityList WHERE (TrINQDTId = @Id) ORDER BY Id",
                 new
                 {
                     Id
                 })).ToList();

                // Fetch sub-activities asynchronously
                var subActivities = (await connection.QueryAsync<PLMSTrActivity>(@"
                SELECT  Id, ActivityId, ActivityName, RequiredDate, PlanRemakrs, ActualCompletedDate, Remarks, DoneBy, IsCompleted, ZipFilePath
                FROM       PLMS_VwTrActivitySubList WHERE (TrINQDTId = @Id) ORDER BY Id",
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

        public async Task<Models.PLMS.NewInquiryVM> LoadInquiryDropDowns()
        {
            var oInquiriesVM = new Models.PLMS.NewInquiryVM
            {
                InquiryTypesList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoInquiryTypes"),
                CustomersList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoCustomers"),
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
