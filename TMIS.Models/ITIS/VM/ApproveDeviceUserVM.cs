using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class ApproveDeviceUserVM
    {
        public int AssignmentID {  get; set; }
        public int DeviceID {  get; set; }
        public string DeviceType {  get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string EmpName {  get; set; } = string.Empty;
        public string AssignedDate { get; set; } = string.Empty;
        public string ApproverEMPNo { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;    
        public string DepartmentName { get; set; } = string.Empty;
    }
}
