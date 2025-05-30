namespace TMIS.Models.TGPS.VM
{
    public class GoodsPassList
    {
        public int Id { get; set; }
        public string GatePassNo { get; set; } = string.Empty;
        public string GenDateTime { get; set; } = string.Empty;
        public string GenGPassTo { get; set; } = string.Empty;
        public string GpSubject { get; set; } = string.Empty;
        public string PassStatus { get; set; } = string.Empty;
    }
}
