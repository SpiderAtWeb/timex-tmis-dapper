namespace TMIS.Models.ITIS.VM
{
    public class ViewDeviceVM
    {
        public DeviceDetailVM? DeviceDetail { get; set; } = new DeviceDetailVM();
        public DeviceUserDetailVM? DeviceUserDetail { get; set; } = new DeviceUserDetailVM();
    }
}
