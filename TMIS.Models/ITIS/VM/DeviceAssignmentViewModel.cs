using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class DeviceAssignmentViewModel
    {
        public string? EMPNo { get; set; }
        public string? EmpName { get; set; }
        public string? UserStatus { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? AssignLocation { get; set; }
        public string? AssignDepartment { get; set; }
        public string? Designation { get; set; }
        public string? AssignRemarks { get; set; }
        public string? AssignBy { get; set; }
        public string? ReturnRemarks { get; set; }
    }
}
