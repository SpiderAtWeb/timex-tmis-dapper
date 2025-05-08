using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface IDatabaseConnectionAdm
    {
        IDbConnection GetConnection();
    }
}
