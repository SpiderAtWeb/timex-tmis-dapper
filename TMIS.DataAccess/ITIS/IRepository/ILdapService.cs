using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface ILdapService
    {
        Task<IEnumerable<SelectListItem>> GetEmployeesFromAD();
    }
}
