using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.TAPS;

namespace TMIS.DataAccess.TAPS.IRepository
{
    public interface ITAPSLogdbRepository
    {
        public void InsertLog(TAPSLogdb log);
    }
}
