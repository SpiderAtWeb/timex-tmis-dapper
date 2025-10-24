using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TPMS.VM
{
    public class PurchaseVM
    {
        public int RequestID { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? UserName { get; set; }
        public string? Requirement { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? Unit { get; set; }
        public int? QTY { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? RequestBy { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? RequestStatus { get; set; }
    }
}
