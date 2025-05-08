using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.COMON.IRpository
{
    public interface ISessionHelper
    {
        void SetUserSession(string userId, string nameWi, string userRole, int[] accessPlants);
        string GetUserId();

        string GetUserName();

        string GetUserRole();

        string[] GetAccessPlantsArray();

        void ClearSession();

    }
}
