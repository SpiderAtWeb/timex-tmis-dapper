using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface ISessionHelper
    {
        void SetUserSession(string userId, string shortName,string[] userRole, int[] userLocList);

        string GetUserId();

        string GetShortName();

        string[] GetUserRolesList();

        string[] GetLocationList();

        void ClearSession();

    }
}
