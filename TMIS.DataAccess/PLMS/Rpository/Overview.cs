using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class Overview(IDatabaseConnectionSys dbConnection) : IOverview
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        public async Task<IEnumerable<PendingActivity>> GetAllRunningInqsData()
        {
            string query = @"
              SELECT [Id]
                  ,CONCAT(InquiryRef, '-', CycleNo) AS InquiryRef
                  ,[Customer]
                  ,[CycleNo]
                  ,[StyleNo]
                  ,[StyleDesc]
	              ,[ColorCode]
                  ,[InquiryType]
                  ,[ResponseType]
                  ,[Seasons]
                  ,[SampleType]
                  ,[SampleStage]
                  ,CONVERT(VARCHAR(10), [InquiryRecDate], 101) AS [InquiryRecDate]
                  ,CONVERT(VARCHAR(10), [DateResponseReq], 101) AS [DateResponseReq]
                  ,[InquiryComment]
                  ,[IsFOBOrCM]
                  ,[Price]
                  ,[SMV]
                  ,[BuyerComments]
                  ,[DateActualRespRec]    
              FROM [TMIS].[dbo].[PLMS_VwPendActivityList] ORDER BY [InquiryRef] ASC";  // SQL query for paging

            var result = await _dbConnection.GetConnection().QueryAsync<PendingActivity>(query);
            return result;
        }
    }
}
