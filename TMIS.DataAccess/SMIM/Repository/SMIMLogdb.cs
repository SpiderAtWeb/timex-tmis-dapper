using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class SMIMLogdb(IHttpContextAccessor httpCtxtAcsor,
        IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : ISMIMLogdb
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IHttpContextAccessor _httpCtxtAcsor = httpCtxtAcsor;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(IDatabaseConnectionSys dbConnection, Logdb log)
        {
            var sql = @"INSERT INTO [dbo].[SMIM_TrLogger]
                       ([TrDateTime]
                       ,[McId]
                       ,[TrLog]
                       ,[TrUser])
                        VALUES
                       (@TrDateTime
                       ,@McId
                       ,@TrLog
                       ,@TrUser)";

            _dbConnection.GetConnection().Execute(sql, new
            {
                TrDateTime = DateTime.Now,
                McId = log.TrObjectId,
                log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            });
        }

    }
}
