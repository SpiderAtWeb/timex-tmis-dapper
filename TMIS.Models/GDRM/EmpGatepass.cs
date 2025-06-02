namespace TMIS.Models.GDRM
{
    public class EmpGatepass
    {
        public int Id { get; set; }
        public string EmpGpNo { get; set; } = string.Empty;
        public string GateName { get; set; } = string.Empty;
        public string ExpLoc { get; set; } = string.Empty;
        public string ExpReason { get; set; } = string.Empty;
        public DateTime? ExpOutTime { get; set; }
        public string GenUser { get; set; } = string.Empty;
        public DateTime? ResponsedDateTime { get; set; }
        public string ResponsedBy { get; set; } = string.Empty;
        public string IsReturn { get; set; } = string.Empty;
        public List<EmpGatepassDetails> EmpGatepassDetails { get; set; } = new();
    }
}
