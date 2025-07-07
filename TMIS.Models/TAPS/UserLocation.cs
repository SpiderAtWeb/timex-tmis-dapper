using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TAPS
{
    public class UserLocation
    {
        public int UserId { get; set; }
        public int UserLocationId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserLocationDesc { get; set; }
    }
}
