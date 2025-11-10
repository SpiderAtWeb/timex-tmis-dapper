using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class Transporter
    {
        public int TransporterId { get; set; }

        [DisplayName("NIC")]
        public string? NIC { get; set; }

        [DisplayName("Transporter Name")]
        [Required]
        public string TransporterName { get; set; } = string.Empty;

        [DisplayName("Mobile Number")]
        public string? PhoneMobile { get; set; }
        
        [DisplayName("Account No")]
        public string? AccountNo { get; set; }
        
        [DisplayName("Account Holder Name")]
        public string? AccountHolderName { get; set; }

        [DisplayName("Bank")]
        public string? Bank { get; set; }

        [DisplayName("Branch")]
        public string? Branch { get; set; }

        [DisplayName("Bank Code")]
        public string? BankCode { get; set; }

        [DisplayName("Branch Code")]
        public string? BranchCode { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
