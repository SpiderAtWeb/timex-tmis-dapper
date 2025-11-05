using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string NIC { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string LicenseNo { get; set; } = string.Empty;
        public string? PhoneMobile { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }
}
