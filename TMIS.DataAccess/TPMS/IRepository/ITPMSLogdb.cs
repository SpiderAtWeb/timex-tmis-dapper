using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.TPMS;

namespace TMIS.DataAccess.TPMS.IRepository
{
    public interface ITPMSLogdb
    {
        public void InsertLog(IDatabaseConnectionSys dbConnection, TPMS_TrLogger log);
    }
}
