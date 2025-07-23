using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.HRRS;
using TMIS.Models.ITIS;


namespace TMIS.DataAccess.HRRS.IRepository
{
    public interface IHRRSLogdb
    {
        public void InsertLog(IDatabaseConnectionSys dbConnection, LogdbHRRS log);
    }
}
