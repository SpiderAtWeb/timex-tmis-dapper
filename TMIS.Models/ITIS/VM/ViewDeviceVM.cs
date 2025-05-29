using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class ViewDeviceVM
    {
        public DeviceDetailVM? DeviceDetail { get; set; }
        public DeviceUserDetailVM? DeviceUserDetail { get; set; }
    }
}
