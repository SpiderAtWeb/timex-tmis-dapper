using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        [DisplayName("Description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        [DisplayName("SerialNumber")]
        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [Required]
        [DisplayName("User Name")]
        [StringLength(100)]
        public string? UserName { get; set; }

        [Required]
        [DisplayName("Requirement")]
        [StringLength(500)]
        public string? Requirement { get; set; }

        [Required]
        [DisplayName("Department")]
        [StringLength(100)]
        public string? Department { get; set; }

        [Required]
        [DisplayName("Designation")]
        [StringLength(100)]
        public string? Designation { get; set; }

        [Required]
        [DisplayName("Unit")]
        [StringLength(100)]
        public string? Unit { get; set; }

        [Required]
        [DisplayName("QTY")]
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
