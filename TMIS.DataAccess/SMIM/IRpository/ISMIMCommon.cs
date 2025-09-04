using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface ISMIMCommon
    {
        Task<IEnumerable<SelectListItem>> GetUnitsList();
    }
}
