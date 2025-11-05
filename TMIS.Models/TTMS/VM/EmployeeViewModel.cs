using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DestinationName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }        
        public string Location { get; set; } = string.Empty;        
    }
}
