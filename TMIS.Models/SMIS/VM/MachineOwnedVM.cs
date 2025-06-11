using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS.VM
{
    public class MachineOwnedVM
    {
        public int Id { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string FarCode { get; set; } = string.Empty;
        public string DatePurchased { get; set; } = string.Empty;
        public string ServiceSeq { get; set; } = string.Empty;
        public string MachineBrand { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string OwnedUnit { get; set; } = string.Empty;
        public string CurrentUnit { get; set; } = string.Empty;
        public string MachineModel { get; set; } = string.Empty;
        public string Cost { get; set; } = string.Empty;
        public byte[]? ImageFR { get; set; }
        public byte[]? ImageBK { get; set; }
        public string Status { get; set; } = string.Empty;
        public string LastScanDateTime { get; set; } = string.Empty;

    }
}
