namespace TMIS.Models.GDRM
{
    public class EmpHistoryVM
    {
        public string StepLabel { get; set; } = string.Empty;
        public int StepUpdate { get; set; }
        public DateTime? StepDateTime { get; set; }
        public string StepText { get; set; } = string.Empty;
    }
}
