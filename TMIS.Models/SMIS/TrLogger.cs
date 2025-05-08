using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS
{
    public class TrLogger
    {
        public DateTime TrDateTime { get; set; }
        public string TrLog { get; set; } = string.Empty;
        public string TrUser { get; set; } = string.Empty;
    }
}
