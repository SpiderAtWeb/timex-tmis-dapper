namespace TMIS.Models.TGPS.VM
{
    public class GatepassVM
    {
        public List<GatepassAddress> GatepassAddresses { get; set; } = [];
        public List<GatepassItem> Items { get; set; } = [];

        public string GpSubject { get; set; } = string.Empty;
        public string Attention { get; set; } = string.Empty;
        public string SendApprovalTo { get; set; } = string.Empty;
        public bool IsReturnable { get; set; }
        public string ReturnDays { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string BoiGatepass { get; set; } = string.Empty;

    }
}
