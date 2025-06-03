using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class AssignDeviceVM
    {
        [Required]
        [DisplayName("Employee#")]
        public string EmpNo { get; set; } = string.Empty;
        [Required]
        [DisplayName("Employee")]
        public string EmpName { get; set; } = string.Empty;
        [Required]
        [DisplayName("Device")]
        public int Device {  get; set; } 
        [Required]
        [DisplayName("Approver")]
        public string? Approver { get; set; }
        [Required]
        [DisplayName("Designation")]
        public string? Designation { get; set; }
        [Required]
        [DisplayName("Location")]
        public string? AssignLocation {  get; set; }
        [Required]
        [DisplayName("Department")]
        public string? AssignDepartment {  get; set; }
        [DisplayName("Comment")]
        public string? AssignRemark { get;set; }
    }
}
