using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                await DeleteOldActivitySchedules(connection, oActivitySave.Id);

                int headerId = await SaveActivityScheduleHeader(
                                                  connection,
                                                  oActivitySave.CPName);

                foreach (var node in oActivitySave.TreeData!)
                {
                    int scheduleId = await SaveActivitySchedule(
                                                   connection,
                                                   node,
                                                   headerId
                                               );

                    if (node.ActivityList != null && node.ActivityList.Count > 0)
                    {
                        await SaveActivityScheduleSub(connection, scheduleId, node.ActivityList);
                    }
                }
            }
        }

        public async Task DeleteOldActivitySchedules(IDbConnection connection, int Id)
        {
            string deleteSubs = @"
          DELETE FROM dbo.PLMS_CPTemplateScheduleSub
            WHERE EXISTS (
                SELECT 1
                FROM dbo.PLMS_CPTemplateSchedule
                INNER JOIN dbo.PLMS_CPTemplateHeader
                    ON dbo.PLMS_CPTemplateSchedule.CpHeaderId = dbo.PLMS_CPTemplateHeader.Id
                WHERE dbo.PLMS_CPTemplateSchedule.Id = PLMS_CPTemplateScheduleSub.CpHeaderSubId
                  AND dbo.PLMS_CPTemplateHeader.Id = @Id
            );";

            string deleteSched = @"
            DELETE FROM dbo.PLMS_CPTemplateSchedule
            WHERE EXISTS (
            SELECT 1
            FROM dbo.PLMS_CPTemplateSchedule INNER JOIN
            dbo.PLMS_CPTemplateHeader ON dbo.PLMS_CPTemplateSchedule.CpHeaderId = dbo.PLMS_CPTemplateHeader.Id
            WHERE (dbo.PLMS_CPTemplateHeader.Id = @Id))";

            string deleteHeader = @"
            DELETE FROM dbo.PLMS_CPTemplateHeader
            WHERE Id = @Id";

            var parameters = new
            {
                Id
            };

            await connection.ExecuteAsync(deleteSubs, parameters);
            await connection.ExecuteAsync(deleteSched, parameters);
            await connection.ExecuteAsync(deleteHeader, parameters);
        }

        private async Task<int> SaveActivityScheduleHeader(IDbConnection connection, string cpName)
        {
            string insertQuery = @"
                INSERT INTO [dbo].[PLMS_CPTemplateHeader]
               ([CpName])
                VALUES
               (@CpName);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                CpName = cpName
            };
            return await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
        }

        private async Task<int> SaveActivitySchedule(IDbConnection connection, TreeNode node, int activityHeaderId)
        {
            string insertQuery = @"INSERT INTO [dbo].[PLMS_CPTemplateSchedule]
                   ([CpHeaderId]
                   ,[UserCategoryId]
                   ,[ActivityId]
                   ,[DaysCount]
                   ,[IsAwaitTask])
           VALUES
                   (@CpHeaderId
                   ,@UserCategoryId
                   ,@ActivityId
                   ,@DaysCount
                   ,@IsAwaitTask);
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                CpHeaderId = activityHeaderId,
                node.UserCategoryId,
                node.ActivityId,
                DaysCount = ParseDays(node.Days),
                node.IsAwaitTask
            };
            return await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
        }

        private async Task SaveActivityScheduleSub(IDbConnection connection, int parentScheduleId, List<TreeNode> childNodes)
        {
            const string insertQuery = @"
            INSERT INTO PLMS_CPTemplateScheduleSub 
                        ([CpHeaderSubId], 
                        [UserCategoryId], 
                        [SubActivityId], 
                        [DaysCount],
                        [IsAwaitTask])
            VALUES (@CpHeaderSubId, @UserCategoryId, @SubActivityId, @DaysCount, @IsAwaitTask);";

            foreach (var child in childNodes)
            {
                var parameters = new
                {
                    CpHeaderSubId = parentScheduleId,
                    child.UserCategoryId,
                    SubActivityId = child.ActivityId,
                    DaysCount = ParseDays(child.Days),
                    child.IsAwaitTask
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

        public async Task<List<PLMSActivity>> LoadSavedActivityList(int id)
        {
            // Create the dynamic model
            var dynamicModel = new List<PLMSActivity>();
            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    // Fetch activities asynchronously
                    var activities = (await connection.QueryAsync<PLMSActivity>(@"
                    SELECT ActivityId, UserCategoryId, UserCategoryText, Days, ActivityText, IsAwaitTask
                    FROM PLMS_VwActivityList
                    WHERE  (CpHeaderId = @ReacordId) ORDER BY Id",
                     new
                     {
                         ReacordId = id
                     })).ToList();


                    // Fetch sub-activities asynchronously
                    var subActivities = (await connection.QueryAsync<PLMSActivity>(@"
                    SELECT      ActivityId, SubActivityId, Days, UserCategoryId, UserCategoryText, SubActivityText, IsAwaitTask
                    FROM            PLMS_VwActivitySubList
                    WHERE (CpHeaderId = @ReacordId) ORDER BY Id",
                 new
                 {
                     ReacordId = id
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
                            IsAwaitTask = activity.IsAwaitTask,
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
                                    UserCategoryText = subActivity.UserCategoryText,
                                    IsAwaitTask = subActivity.IsAwaitTask,
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

            string query = $@"SELECT CAST(Id AS NVARCHAR) AS Value, 
            CpName AS Text FROM PLMS_CPTemplateHeader ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);

            var oInquiriesVM = new CPathDataVM
            {
                DropActivityList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoActivityTypes"),
                DropUserCategoryList = await _userControls.LoadDropDownsAsync("PLMS_MasterTwoUserCategories"),
                CPathList = results

            };
            return oInquiriesVM;
        }

    }
}
