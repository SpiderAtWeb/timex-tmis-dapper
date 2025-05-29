using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class DeviceUserVM
    {
        public int DeviceID { get; set; }
        public int AssignmentID { get; set; }
        public string? EmpNo { get; set; }
        public string? EmpName { get; set; }
        public string? Designation { get; set; }
        public string? SerialNumber { get; set; }
        public string? DeviceType { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? LocationName { get; set; }
        public string? DepartmentName{ get; set; }
        public string? DeviceName{ get; set; }        
        public string? AssignStatus{ get; set; }        
    }
}
