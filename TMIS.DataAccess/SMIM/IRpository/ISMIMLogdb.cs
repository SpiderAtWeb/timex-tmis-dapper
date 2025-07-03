using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface ISMIMLogdb
    {
        public void InsertLog(IDbConnection dbConnection, Logdb log, IDbTransaction transaction);      
    }
}
