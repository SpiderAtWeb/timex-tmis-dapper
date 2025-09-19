using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class Feedback(IDatabaseConnectionSys dbConnection, IPLMSCommon common, IPLMSLogdb pLMSLogdb) : IFeedback
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IPLMSCommon _common = common;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<FeedbackVM> GetInquiryAsync(string id)
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '.v', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResponseType, Customer, Seasons, SampleType, SampleStage, InquiryComment
                   FROM PLMS_VwInqListPending WHERE Id = @Id";
            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    var captionData = await connection.QuerySingleOrDefaultAsync<ShowInquiryDataVM>(sql, new { Id = id }) ?? throw new KeyNotFoundException($"No inquiry found for Id: {id}");
                    var activistList = _common.LoadModalDataAsync(id);

                    var feedbackVM = new FeedbackVM
                    {
                        ShowInqDataVM = captionData,
                        ModalShowVM = await activistList
                    };
                    return feedbackVM;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new InvalidOperationException($"Error fetching inquiry for Id: {id}", ex);
            }
        }

        public async Task<string> SaveFeedbackAsync(int id, string buyerComment, int actionType)
        {
            string query = @"
            SELECT COUNT(TaskId) AS PendingActivities
            FROM [dbo].[PLMS_VwTrActivityList]
            WHERE [Id] = @Id AND [ActivityIsCompleted] = 0";

            string queryUpdate = @"
            UPDATE [dbo].[PLMS_TrInqDetails]
            SET [IsApproved] = @IsApproved,
                [BuyerComments] = @BuyerComments,
                [DateActualRespRec] = @DateActualRespRec
            WHERE [Id] = @Id";

            using (var connection = _dbConnection.GetConnection())
            {
                int pendingActivities = await connection.ExecuteScalarAsync<int>(query, new { Id = id });

                if (pendingActivities > 0)
                {
                    return "Cannot proceed with the action. There are pending activities";
                }

                // Proceed with the update if no pending activities
                var parameters = new
                {
                    IsApproved = actionType, // 1 for Approve, 2 for Reject
                    BuyerComments = buyerComment,
                    DateActualRespRec = DateTime.Now,
                    Id = id
                };

                Logdb logdb = new()
                {
                    TrObjectId = id,
                    TrLog = "CUSTOMER FEEDBACK UPDATED"
                };

                await _pLMSLogdb.InsertLog(_dbConnection, logdb);

                int rowsAffected = await connection.ExecuteAsync(queryUpdate, parameters);

                // Return success or failure message
                return rowsAffected > 0 ? "Update successful" : "Update failed";
            }
        }

        public async Task<int> CheckPendingActivities(int id)
        {
            string query = @"
            SELECT COUNT(TaskId) AS PendingActivities
            FROM [dbo].[PLMS_VwTrActivityList]
            WHERE [Id] = @Id AND [ActivityIsCompleted] = 0";

            using (var connection = _dbConnection.GetConnection())
            {
                int pendingActivities = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
                return pendingActivities;
            }
        }
    }
}
