using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS
{
    public class MachineInventoryResult
    {
        public string GroupName { get; set; } = string.Empty;
        public string Cluster { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string OwnershipStatus { get; set; } = string.Empty;
        public string McType { get; set; } = string.Empty;
        public int McCount { get; set; }
    }
}
