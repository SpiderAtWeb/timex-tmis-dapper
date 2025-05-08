using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS
{
    public class TransMCUser
    {
        public int Id { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;
        public string CurrentUnit { get; set; } = string.Empty;
        public string TrStatusId { get; set; } = string.Empty;

    }
}
