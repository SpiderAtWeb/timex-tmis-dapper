using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TPMS
{
    public class TPMS_PurchaseRequests
    {
        public int RequestID { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(500)]
        public string? Requirement { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        [StringLength(100)]
        public string? Unit { get; set; }

        public int? QTY { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RequestDate { get; set; }

        [StringLength(20)]
        public string? RequestBy { get; set; }

        public int? RequestStatus { get; set; }
        public bool? IsDelete { get; set; }
       
        [Column(TypeName = "datetime")]
        public DateTime? PurchaseDate { get; set; }
    }
}
