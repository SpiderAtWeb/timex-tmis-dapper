using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.ITIS.VM
{
    public class ViewDeviceVM
    {
        public DeviceDetailVM? DeviceDetail { get; set; } = new DeviceDetailVM();
        public DeviceUserDetailVM? DeviceUserDetail { get; set; } = new DeviceUserDetailVM();
        public IEnumerable<DeviceUserDetailVM>? PreviousUsers { get; set; }
    }
}
