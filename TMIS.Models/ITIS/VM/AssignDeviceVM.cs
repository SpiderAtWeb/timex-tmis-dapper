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
        [DisplayName("Employee")]
        public string EmpNo { get; set; }
        [Required]
        [DisplayName("Device")]
        public int Device {  get; set; }
        [Required]
        [DisplayName("Approver")]
        public string Approver { get; set; }
    }
}
