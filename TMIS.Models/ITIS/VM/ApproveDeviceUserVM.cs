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
        public string SerialNumber { get; set; } = string.Empty;
        public string EmpName {  get; set; } = string.Empty;
        public string AssignedDate { get; set; } = string.Empty;
        public string ApproverEMPNo { get; set; } = string.Empty;
    }
}
