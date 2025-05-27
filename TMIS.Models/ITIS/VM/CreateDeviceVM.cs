using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.ITIS.VM
{
    public class CreateDeviceVM
    {
        public IEnumerable<SelectListItem>? DeviceTypeList { get; set; }
        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? DeviceStatusList { get; set; }
        public IEnumerable<SelectListItem>? VendorsList { get; set; }
        public Device? Device { get; set; }
        public List<AttributeWithOptionsVM>? Attributes { get; set; }
    }
}
