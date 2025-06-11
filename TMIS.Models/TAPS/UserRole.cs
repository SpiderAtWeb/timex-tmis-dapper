using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TAPS
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRoleDesc { get; set; }
    }
}
