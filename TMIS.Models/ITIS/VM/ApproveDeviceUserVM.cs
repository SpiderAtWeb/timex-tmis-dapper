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
        public string SerialNumber { get; set; }
        public string EmpName {  get; set; }
        public string AssignedDate { get; set; }
        public string ApproverEMPNo { get; set; }
    }
}
