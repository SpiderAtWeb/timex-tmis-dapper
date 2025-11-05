using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class NewEmployeeViewModel
    {
        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? DestinationList { get; set; }
        public Employee Employee { get; set; } = new Employee();
    }
}
