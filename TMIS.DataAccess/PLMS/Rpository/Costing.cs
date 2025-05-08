using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class Costing(IDatabaseConnectionSys dbConnection, ICommon common, IPLMSLogdb pLMSLogdb) : ICosting
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommon _common = common;
        private readonly IPLMSLogdb _pLMSLogdb = pLMSLogdb;

        public async Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync()
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResposeType, Customer, Seasons, SampleType, SampleStage, InquiryComment
                   FROM PLMS_VwInqCostPending WHERE (IsPriceStageAv = 1) AND (IsPriceUpdate = 0)";

            return await _dbConnection.GetConnection().QueryAsync<ShowInquiryDataVM>(sql);
        }

        public async Task<FeedbackVM> GetInquiryAsync(string id)
        {
            string sql = @"SELECT Id, CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef, CycleNo, StyleNo, StyleDesc, ColorCode, 
                          InquiryType, ResposeType, Customer, Seasons, SampleType, SampleStage, InquiryComment
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

        public async Task<string> SaveCosting(int id, string costPrice, string priceComment, string fob)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                var query = @"UPDATE [dbo].[PLMS_TrInqDetails]
                   SET [IsPriceUpdate] = 1
                      ,[IsFOBOrCM] = @IsFOBOrCM
                      ,[Price] = @Price
                      ,[PriceComment] = @PriceComment
                      ,[PriceUpdateDate] = @PriceUpdateDate
                 WHERE [Id] = @Id";

                var parameters = new
                {
                    Price = costPrice,
                    IsFOBOrCM = fob == "1" ? "FOB" : "CM",
                    priceComment,
                    PriceUpdateDate = DateTime.Now,
                    id

                };

                Logdb logdb = new()
                {
                    TrObjectId = id,
                    TrLog = (fob == "1" ? "FOB" : "CM") + " PRICE " + costPrice +" UPDATED"
                };

                await _pLMSLogdb.InsertLog(_dbConnection, logdb);

                var rowsAffected = await connection.ExecuteAsync(query, parameters); // Dapper's Execute method
                return rowsAffected > 0 ? "Update successful" : "Update failed";
            }
        }
    }
}
