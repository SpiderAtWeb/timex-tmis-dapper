using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class TransporterViewModel
    {
        public int TransporterId { get; set; }
        public string NIC { get; set; } = string.Empty;
        public string TransporterName { get; set; } = string.Empty;
        public string? PhoneMobile { get; set; }
        public string? AccountHolderName { get; set; }
        public string? Bank { get; set; }
        public string? Branch { get; set; }
        public string? BankCode { get; set; }
        public string? BranchCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
