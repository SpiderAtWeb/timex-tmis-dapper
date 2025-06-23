using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.ITIS.VM
{
    public class ReturnDeviceVM
    {
        public IEnumerable<SelectListItem>? DeviceSerialList { get; set; }
        public DeviceDetailVM? DeviceDetail { get; set; } = new DeviceDetailVM();
        [Required]
        [DisplayName("Comment")]
        public string? ReturnRemark {  get; set; }
        [Required]
        [DisplayName("Device")]
        public string? Device { get; set; }
        public int RecordID {  get; set; }
        public DeviceUserDetailVM? DeviceUserDetail { get; set; } = new DeviceUserDetailVM();
        [DisplayName("Return Time Image")]
        public byte[]? ReturnTimeImage { get; set; }
    }
}
