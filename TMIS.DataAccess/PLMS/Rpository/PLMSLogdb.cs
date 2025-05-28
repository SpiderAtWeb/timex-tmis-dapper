using Dapper;
using System.Data;
using System.Diagnostics;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.Rpository
{
    public class PLMSLogdb(IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : IPLMSLogdb
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task InsertLog(IDatabaseConnectionSys dbConnection, Logdb log)
        {
            var sql = @"INSERT INTO [dbo].[PLMS_TrLogger]
                       ([TrDateTime]
                       ,[InqId]
                       ,[TrLog]
                       ,[TrUser])
                        VALUES
                       (@TrDateTime
                       ,@InqId
                       ,@TrLog
                       ,@TrUser)";

            await _dbConnection.GetConnection().ExecuteAsync(sql, new
            {
                TrDateTime = DateTime.Now,
                InqId = log.TrObjectId,
                log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            });
        }

        public async Task<int> InsertLogTrans(IDbConnection connection, IDbTransaction transaction, Logdb log)
        {

            string val = _iSessionHelper.GetShortName();


            var sql = @"INSERT INTO [dbo].[PLMS_TrLogger]
                       ([TrDateTime]
                       ,[InqId]
                       ,[TrLog]
                       ,[TrUser])
                        VALUES
                       (@TrDateTime
                       ,@InqId
                       ,@TrLog
                       ,@TrUser)";

            return await connection.ExecuteAsync(sql, new
            {
                TrDateTime = DateTime.Now,
                InqId = log.TrObjectId,
                log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            }, transaction);

        }

    }
}
