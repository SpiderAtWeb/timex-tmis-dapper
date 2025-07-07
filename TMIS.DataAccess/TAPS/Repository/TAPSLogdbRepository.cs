using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TAPS.IRepository;
using TMIS.Models.TAPS;

namespace TMIS.DataAccess.TAPS.Repository
{
    public class TAPSLogdbRepository(IHttpContextAccessor httpCtxtAcsor,
        IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : ITAPSLogdbRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IHttpContextAccessor _httpCtxtAcsor = httpCtxtAcsor;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(TAPSLogdb log)
        {
            var sql = @"INSERT INTO [dbo].[TAPS_TrLogger]
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
