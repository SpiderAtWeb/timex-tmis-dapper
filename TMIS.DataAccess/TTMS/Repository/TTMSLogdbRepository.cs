using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TTMS.IRepository;
using TMIS.Models.TTMS;

namespace TMIS.DataAccess.TTMS.Repository
{
    public class TTMSLogdbRepository(IHttpContextAccessor httpCtxtAcsor, IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : ITTMSLogdbRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IHttpContextAccessor _httpCtxtAcsor = httpCtxtAcsor;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(IDatabaseConnectionSys dbConnection, LogdbTTMS log)
        {
            var sql = @"INSERT INTO [dbo].[TTMS_TrLogger]
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
                RefID = log.TrObjectId,
                TrLog = log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            });
        }
    }
}
