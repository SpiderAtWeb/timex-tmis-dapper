namespace TMIS.Models.TGPS.VM
{
    public class GpHistoryVM
    {
        public int StepOrder { get; set; }
        public string StepLabel { get; set; } = string.Empty;
        public DateTime? StepDateTime { get; set; }
        public string StepText { get; set; } = string.Empty;
        public int StepUpdate { get; set; }
    }
}
