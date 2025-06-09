using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.DataAccess.TAPS.IRepository
{
    public interface ILdapServiceRepository
    {
        Task<bool> GetEmployeesFromAD();
        Task<bool> ButtonStatus(string buttonName);
    }
}
