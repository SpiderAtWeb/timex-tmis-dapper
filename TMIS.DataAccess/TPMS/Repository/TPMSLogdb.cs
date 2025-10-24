using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TPMS.IRepository;
using TMIS.Models.TPMS;

namespace TMIS.DataAccess.TPMS.Repository
{
    public class TPMSLogdb(IHttpContextAccessor httpCtxtAcsor,
        IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : ITPMSLogdb
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;        
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(IDatabaseConnectionSys dbConnection, TPMS_TrLogger log)
        {
            var sql = @"INSERT INTO [dbo].[TPMS_TrLogger]
                       ([TrDateTime]
                       ,[RefID]
                       ,[TrLog]
                       ,[TrUser])
                        VALUES
                       (@TrDateTime
                       ,@RefID
                       ,@TrLog
                       ,@TrUser)";

            _dbConnection.GetConnection().Execute(sql, new
            {
                TrDateTime = DateTime.Now,
                RefID = log.RefID,
                TrLog = log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            });
        }
    }
}
