using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS.VM
{
    public class VMCluster
    {
        public int OwnedClusterId { get; set; }

        public IEnumerable<SelectListItem>? OwnedClusterList { get; set; }
    }
}
