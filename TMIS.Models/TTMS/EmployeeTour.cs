using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class EmployeeTour
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Dictionary<DateTime, bool> Attendance { get; set; }

        public EmployeeTour()
        {
            EmployeeCode = string.Empty;
            EmployeeName = string.Empty;
            Attendance = [];
        }
    }
}
