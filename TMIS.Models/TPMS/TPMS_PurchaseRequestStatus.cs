using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TPMS
{
    public class TPMS_PurchaseRequestStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PropName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PropDesc { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateCreate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateUpdate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateDelete { get; set; }

        [Required]
        public bool IsDelete { get; set; }
    }
}
