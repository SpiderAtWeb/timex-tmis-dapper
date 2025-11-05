using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.TTMS;

namespace TMIS.DataAccess.TTMS.IRepository
{
    public interface ITTMSLogdbRepository
    {
        public void InsertLog(IDatabaseConnectionSys dbConnection, LogdbTTMS log);
    }
}
