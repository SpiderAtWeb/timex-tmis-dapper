using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IITISLogdb
    {
        public void InsertLog(IDatabaseConnectionSys dbConnection, Logdb log);
    }
}
