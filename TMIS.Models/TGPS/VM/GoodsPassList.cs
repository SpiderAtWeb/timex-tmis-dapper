using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TGPS.VM
{
   public class GoodsPassList
    {
        public int Id { get; set; }
        public string GatePassNo { get; set; } = string.Empty;
        public string GenDateTime { get; set; } = string.Empty;
        public string GenGPassTo { get; set; } = string.Empty;
        public string GpSubject { get; set; } = string.Empty;
        public string PassStatus { get; set; } = string.Empty;
    }
}
