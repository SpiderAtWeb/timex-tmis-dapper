using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class PLMSLogdb(IHttpContextAccessor httpCtxtAcsor,
        IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : IPLMSLogdb
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IHttpContextAccessor _httpCtxtAcsor = httpCtxtAcsor;
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
                TrUser = _iSessionHelper.GetUserName().ToUpper(),
            });
        }

        public async Task InsertLogTrans(IDbConnection connection, IDbTransaction transaction, Logdb log)
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

            await connection.ExecuteAsync(sql, new
            {
                TrDateTime = DateTime.Now,
                InqId = log.TrObjectId,
                log.TrLog,
                TrUser = _iSessionHelper.GetUserName().ToUpper(),
            }, transaction);
        }

    }
}
