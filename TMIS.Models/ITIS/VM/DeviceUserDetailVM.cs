using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class DeviceUserDetailVM
    {
        public int AssignmentID {  get; set; }
        public string? EmpNo {  get; set; }
        public string? EmpName { get; set; }
        public string? Designation {  get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? AssignRemarks {  get; set; }
        public string? ApproverEMPNO {  get; set; }
        public DateTime? ApproverResponseDate { get; set; }
        public string? ApproverRemark { get; set; }
        public byte[]? AssignTimeImage {  get; set; }   
        public string? AssignDepartment {  get; set; }
        public string? AssignLocation {  get; set; }
        public string? AssignStatus {  get; set; }
    }
}
