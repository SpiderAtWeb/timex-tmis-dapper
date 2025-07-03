namespace TMIS.Models.PLMS
{
    public class PLMSTrActivity
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string RequiredDate { get; set; } = string.Empty;
        public string PlanRemakrs { get; set; } = string.Empty;
        public string ActualCompletedDate { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string DoneBy { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int DueDates { get; set; }
        public string ZipFilePath { get; set; } = string.Empty;

        public List<PLMSTrActivity>? SubActivityList { get; set; }
    }
}
