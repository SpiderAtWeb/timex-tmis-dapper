using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using log4net.Util;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.HRRS.Repository
{
    public class HRRSLogdb(IHttpContextAccessor httpCtxtAcsor,
        IDatabaseConnectionSys dbConnection,
        ISessionHelper sessionHelper) : IHRRSLogdb
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IHttpContextAccessor _httpCtxtAcsor = httpCtxtAcsor;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(IDatabaseConnectionSys dbConnection, Logdb log)
        {
            var sql = @"INSERT INTO [dbo].[HRRS_TrLogger]
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
