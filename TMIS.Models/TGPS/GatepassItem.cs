using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TGPS
{
    public class GatepassItem
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ItemUnit { get; set; }
    }
}
