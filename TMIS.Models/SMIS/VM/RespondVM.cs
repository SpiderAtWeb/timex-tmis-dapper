using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS.VM
{
    public class RespondVM
    {
        public int Id { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;
        public string ReqUnit { get; set; } = string.Empty;
        public string ReqLocation { get; set; } = string.Empty;
        public string TrUserId { get; set; } = string.Empty;
        public string ReqRemark { get; set; } = string.Empty;
        public string DateCreate { get; set; } = string.Empty;

    }
}
