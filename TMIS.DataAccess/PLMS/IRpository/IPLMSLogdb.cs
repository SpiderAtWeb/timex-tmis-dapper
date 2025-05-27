using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface IPLMSLogdb
    {
        public Task InsertLog(IDatabaseConnectionSys dbConnection, Logdb log);

        public Task<int> InsertLogTrans(IDbConnection connection, IDbTransaction transaction, Logdb log);
    }
}
