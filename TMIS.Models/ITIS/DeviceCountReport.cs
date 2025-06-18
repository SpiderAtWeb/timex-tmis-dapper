using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class DeviceCountReport
    {
        public int DeviceID { get; set; }
        public string? Location { get; set; }
        public string? DeviceType { get; set; }
        public string? Status { get; set; }
    }
}
