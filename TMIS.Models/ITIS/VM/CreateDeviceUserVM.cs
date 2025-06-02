using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.ITIS.VM
{
    public class CreateDeviceUserVM
    {
        public IEnumerable<SelectListItem>? DeviceSerialList { get; set; }
        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
        public IEnumerable<SelectListItem>? ApproverList { get; set; }
        public IEnumerable<SelectListItem>? DepartmentList { get; set; }
        public DeviceDetailVM? DeviceDetail { get; set; } =   new DeviceDetailVM();
        public AssignDeviceVM? AssignDevice {  get; set; }  = new AssignDeviceVM(); 

    }
}
