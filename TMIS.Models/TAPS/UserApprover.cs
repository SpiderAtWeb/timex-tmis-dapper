using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TAPS
{
    public class UserApprover
    {
        public int UserId { get; set; }
        public int AppUserId { get; set; }
        public string? UserEmail { get; set; }
        public string? ApproverEmail { get; set; }
        public string? SystemType { get; set; }
        public string? Approvername { get; set; }
        public string? Username { get; set; }
    }
}
