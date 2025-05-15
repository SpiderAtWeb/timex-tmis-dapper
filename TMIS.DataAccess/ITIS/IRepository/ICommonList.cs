using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface ICommonList
    {
        Task<IEnumerable<SelectListItem>> LoadAttributeTypes();
        Task<IEnumerable<SelectListItem>> LoadDeviceTypes();
        
    }
}
