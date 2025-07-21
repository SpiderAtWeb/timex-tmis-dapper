using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.ITIS.VM;

namespace TMIS.Models.HRRS.VM
{
    public class ITRequestPageViewModel
    {
        public Create CreateObj { get; set; } = new();
        public IEnumerable<HRRS_ITRequest>? ITRequestTableObj { get; set; }
    }
}
