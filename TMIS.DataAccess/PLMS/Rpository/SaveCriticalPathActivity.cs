using Dapper;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class SaveCriticalPathActivity(IDatabaseConnectionSys dbConnection, IUserControls userControls) : ISaveCriticalPathActivity
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IUserControls _userControls = userControls;

        public async Task SaveActivities(ActivitySave oActivitySave)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await DeleteOldActivitySchedules(connection, oActivitySave.SelectedInqTypeId,
                                                   oActivitySave.SelectedRepTypeId,
                                                   oActivitySave.SelectedCustomerId,
                                                   oActivitySave.SelectedSampTypeId,
                                                   oActivitySave.SelectedSampStageId);

                int headerId = await SaveActivityScheduleHeader(
                                                  connection,   
                                                  oActivitySave.SelectedInqTypeId,
                                                  oActivitySave.SelectedRepTypeId,
                                                  oActivitySave.SelectedCustomerId,
                                                  oActivitySave.SelectedSampTypeId,
                                                  oActivitySave.SelectedSampStageId
                                              );

                foreach (var node in oActivitySave.TreeData!)
                {
                    int activityScheduleId = await SaveActivitySchedule(
                                                   connection,
                                                   node,
                                                   headerId
                                               );

                    if (node.ActivityList != null && node.ActivityList.Count > 0)
                    {
                        await SaveActivityScheduleSub(connection, activityScheduleId, node.ActivityList);
                    }
                }
            }
        }

        public async Task DeleteOldActivitySchedules(
            IDbConnection connection,
            int inquiryTypeId,
            int responseTypeId,
            int customerId,
            int sampleTypeId,
            int sampleStageId)
        {
            string deleteSubs = @"
            DELETE sub
            FROM dbo.PLMS_HpActivityScheduleSub sub
            INNER JOIN dbo.PLMS_HpActivitySchedule sched ON sub.ActivitySchedId = sched.Id
            INNER JOIN dbo.PLMS_HpActivityScheduleHeader head ON sched.ActivityHeaderId = head.Id
            WHERE head.InquiryTypeId = @InquiryTypeId
              AND head.ResponseTypeId = @ResponseTypeId
              AND head.CustomerId = @CustomerId
              AND head.SampleTypeId = @SampleTypeId
              AND head.SampleStageId = @SampleStageId";

            string deleteSched = @"
            DELETE sched
            FROM dbo.PLMS_HpActivitySchedule sched
            INNER JOIN dbo.PLMS_HpActivityScheduleHeader head ON sched.ActivityHeaderId = head.Id
            WHERE head.InquiryTypeId = @InquiryTypeId
              AND head.ResponseTypeId = @ResponseTypeId
              AND head.CustomerId = @CustomerId
              AND head.SampleTypeId = @SampleTypeId
              AND head.SampleStageId = @SampleStageId";

            string deleteHeader = @"
            DELETE FROM dbo.PLMS_HpActivityScheduleHeader
            WHERE InquiryTypeId = @InquiryTypeId
              AND ResponseTypeId = @ResponseTypeId
              AND CustomerId = @CustomerId
              AND SampleTypeId = @SampleTypeId
              AND SampleStageId = @SampleStageId";

            var parameters = new
            {
                InquiryTypeId = inquiryTypeId,
                ResponseTypeId = responseTypeId,
                CustomerId = customerId,
                SampleTypeId = sampleTypeId,
                SampleStageId = sampleStageId
            };

            await connection.ExecuteAsync(deleteSubs, parameters);
            await connection.ExecuteAsync(deleteSched, parameters);
            await connection.ExecuteAsync(deleteHeader, parameters); 
        }

        private async Task<int> SaveActivityScheduleHeader(IDbConnection connection, int inquiryTypeId, int responseTypeId, int customerId, int sampleTypeId, int sampleStageId)
        {
            string insertQuery = @"
                INSERT INTO [dbo].[PLMS_HpActivityScheduleHeader]
               ([InquiryTypeId]
               ,[ResponseTypeId]
               ,[CustomerId]
               ,[SampleTypeId]
               ,[SampleStageId])
                VALUES
               (@InquiryTypeId
               ,@ResponseTypeId
               ,@CustomerId
               ,@SampleTypeId
               ,@SampleStageId);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
          
            var parameters = new
            {
                InquiryTypeId = inquiryTypeId,
                ResponseTypeId = responseTypeId,
                CustomerId = customerId,
                SampleTypeId = sampleTypeId,
                SampleStageId = sampleStageId
            };
            return await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
        }

        private async Task<int> SaveActivitySchedule(IDbConnection connection, TreeNode node, int activityHeaderId)
        {
            string insertQuery = @"INSERT INTO [dbo].[PLMS_HpActivitySchedule]
                   ([ActivityHeaderId]
                   ,[UserCategoryId]
                   ,[ActivityId]
                   ,[DaysCount])
           VALUES
                   (@ActivityHeaderId
                   ,@UserCategoryId
                   ,@ActivityId
                   ,@DaysCount);
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                ActivityHeaderId = activityHeaderId,
                node.UserCategoryId,
                node.ActivityId,
                DaysCount = ParseDays(node.Days)
            };
            return await connection.ExecuteScalarAsync<int>(insertQuery, parameters);         
        }

        private async Task SaveActivityScheduleSub(IDbConnection connection, int parentScheduleId, List<TreeNode> childNodes)
        {
            const string insertQuery = @"
            INSERT INTO PLMS_HpActivityScheduleSub (ActivitySchedId, UserCategoryId, SubActivityId, DaysCount)
            VALUES (@ActivitySchedId, @UserCategoryId, @SubActivityId, @DaysCount);";

            foreach (var child in childNodes)
            {
                var parameters = new
                {
                    ActivitySchedId = parentScheduleId,
                    child.UserCategoryId,
                    SubActivityId = child.ActivityId,
                    DaysCount = ParseDays(child.Days)
                };

                await connection.ExecuteAsync(insertQuery, parameters);             
            }
        }

        private int ParseDays(string days)
        {
            if (int.TryParse(days.Split(' ')[0], out int result))
            {
                return result;
            }
            return 0;
        }

        public async Task<List<PLMSActivity>> LoadSavedActivityList(InquiryParams inqParas)
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
            var dynamicModel = new List<PLMSActivity>();
            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    reacordId = await connection.ExecuteScalarAsync<int>(checkIdQuery, parameters);
                    if (reacordId == 0) {
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
                    var subActivities = (await connection.QueryAsync<PLMSActivity>(@"
                    SELECT      ActivityId, SubActivityId, Days, UserCategoryId, UserCategoryText, SubActivityText
                    FROM            PLMS_VwActivitySubList
                    WHERE (ActivityHeaderId = @ReacordId) ORDER BY Id",
                 new
                 {
                     ReacordId = reacordId
                 })).ToList();

                    // Group sub-activities by ActivityId
                    var subActivityGroups = subActivities
                        .GroupBy(sa => sa.ActivityId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var activity in activities)
                    {
                        var activityModel = new PLMSActivity
                        {
                            ActivityId = activity.ActivityId,
                            ActivityText = activity.ActivityText,
                            Days = activity.Days,
                            UserCategoryId = activity.UserCategoryId,
                            UserCategoryText = activity.UserCategoryText,
                            ActivityList = [] // Initialize the SubActivityList properly
                        };

                        // Check if the activity has associated sub-activities
                        if (subActivityGroups.ContainsKey(activity.ActivityId))
                        {
                            foreach (var subActivity in subActivityGroups[activity.ActivityId])
                            {
                                activityModel.ActivityList.Add(new PLMSActivity
                                {
                                    ActivityId = subActivity.SubActivityId,
                                    ActivityText = subActivity.SubActivityText,
                                    Days = subActivity.Days,
                                    UserCategoryId = subActivity.UserCategoryId,
                                    UserCategoryText = subActivity.UserCategoryText
                                });
                            }
                        }

                        dynamicModel.Add(activityModel);
                    }
                }
            }
            catch
            {
                // Log the exception
                // Example: _logger.LogError(ex, "Error loading saved activity list.");
            }
            return dynamicModel;
        }

        public async Task<CPathDataVM> LoadCPathDropDowns()
        {
            var oInquiriesVM = new CPathDataVM
            {
                InquiryTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdInquiryTypes"),
                ResponseTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdReponseTypes"),
                CustomersList = await _userControls.LoadDropDownsAsync("PLMS_MdCustomers"),
                SeasonsList = await _userControls.LoadDropDownsAsync("PLMS_MdInquirySeason"),
                SampleTypesList = await _userControls.LoadDropDownsAsync("PLMS_MdExtendSub"),
                SampleStagesList = await _userControls.LoadDropDownsAsync("PLMS_MdExtend"),
                DropActivityList = await _userControls.LoadDropDownsAsync("PLMS_MdActivityTypes"),
                DropUserCategoryList = await _userControls.LoadDropDownsAsync("PLMS_MdUserCategories"),
                CPathData = new CPathData()
            };

            return oInquiriesVM;
        }

    }
}
