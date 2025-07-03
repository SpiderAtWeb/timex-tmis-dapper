using Dapper;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class SMIMLogdb(ISessionHelper sessionHelper) : ISMIMLogdb
    {
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public void InsertLog(IDbConnection dbConnection, Logdb log, IDbTransaction transaction)
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

            dbConnection.Execute(sql, new
            {
                TrDateTime = DateTime.Now,
                McId = log.TrObjectId,
                log.TrLog,
                TrUser = _iSessionHelper.GetShortName().ToUpper(),
            }, transaction);
        }

    }
}
