using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class DeviceViewModel
    {
        public int DeviceID { get; set; }
        public string? SerialNumber { get; set; }
        public string? DeviceName { get; set; }
        public string?  DeviceType { get; set; }
        public string? Location { get; set; }
        public string? DeviceStatus { get; set; }
        public List<DeviceAssignmentViewModel> Assignments { get; set; } = new List<DeviceAssignmentViewModel>();
    }
}
