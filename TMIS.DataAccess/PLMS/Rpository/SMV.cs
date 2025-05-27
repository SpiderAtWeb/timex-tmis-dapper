using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class SMV(IDatabaseConnectionSys dbConnection, ICommon common, IPLMSLogdb pLMSLogdb) : ISMV
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommon _common = common;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync()
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResponseType, Customer, Seasons, SampleType, SampleStage, InquiryComment
                 FROM PLMS_VwInqCostPending WHERE (IsSMVStageAv = 1) AND (IsSMVUpdate = 0) ";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql);
        }

        public async Task<FeedbackVM> GetInquiryAsync(string id)
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
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

        public async Task<string> SaveSMV(int id, string smvValue, string smvComment)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                var query = @"UPDATE [dbo].[PLMS_TrInqDetails]
                   SET [IsSMVUpdate] = 1
                      ,[SMV] = @SMV
                      ,[SMVComment] = @smvComment
                      ,[SMVUpdateDate] = @SMVUpdateDate
                 WHERE [Id] = @Id";

                var parameters = new
                {
                    SMV = smvValue,
                    smvComment,
                    SMVUpdateDate = DateTime.Now,
                    id
                };

                Logdb logdb = new()
                {
                    TrObjectId = id,
                    TrLog = smvValue + " SMV UPDATED"
                };

                await _pLMSLogdb.InsertLog(_dbConnection, logdb);

                var rowsAffected = await connection.ExecuteAsync(query, parameters); // Dapper's Execute method
                return rowsAffected > 0 ? "Update successful" : "Update failed";
            }
        }
    }
}
