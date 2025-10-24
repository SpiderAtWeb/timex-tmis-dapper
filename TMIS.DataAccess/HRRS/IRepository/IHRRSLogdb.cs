using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.HRRS;


namespace TMIS.DataAccess.HRRS.IRepository
{
    public interface IHRRSLogdb
    {
        public void InsertLog(IDatabaseConnectionSys dbConnection, LogdbHRRS log);
    }
}
