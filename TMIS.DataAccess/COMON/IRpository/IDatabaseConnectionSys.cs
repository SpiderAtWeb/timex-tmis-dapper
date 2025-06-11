using System.Data;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface IDatabaseConnectionSys
    {
        IDbConnection GetConnection();
    }
}
