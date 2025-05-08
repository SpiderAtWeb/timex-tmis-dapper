using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS.VM
{
    public class PivotDataVM
    {
        public string? Location { get; set; }
        public string? CurrentUnit { get; set; }
        public string? MachineType { get; set; }
        public string? OwnershipStatus { get; set; }
    }
}
