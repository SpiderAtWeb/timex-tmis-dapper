using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TPMS
{
    public class TPMS_TrLogger
    {
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TrDateTime { get; set; }

        public int? RefID { get; set; }

        [StringLength(200)]
        public string? TrLog { get; set; }

        [StringLength(50)]
        public string? TrUser { get; set; }
    }
}
