using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.TGPS.VM;

namespace TMIS.Models.TGPS
{
    public  class EmpPassVM
    {
        public int Id { get; set; }

        public string EmpGpNo { get; set; } = string.Empty;

        public string GateName { get; set; } = string.Empty;

        public string ExpLoc { get; set; } = string.Empty;

        public string ExpReason { get; set; } = string.Empty;

        public string ExpOutTime { get; set; } = string.Empty;

        public string IsReturn { get; set; } = string.Empty;

        public string IsResponsed { get; set; } = string.Empty;

        public string ResponsedBy { get; set; } = string.Empty;
        public string ResponsedDateTime { get; set; } = string.Empty;

        public List<EmpPassEmployees> ShowGPItemVMList { get; set; } = [];

    }
}
