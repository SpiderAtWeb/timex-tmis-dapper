namespace TMIS.Models.PLMS
{
    public class PLMSActivity
    {
        public int ActivityId { get; set; }
        public string ActivityText { get; set; } = string.Empty;

        public int SubActivityId { get; set; }
        public string SubActivityText { get; set; } = string.Empty;

        public string UserCategoryId { get; set; } = string.Empty;
        public string UserCategoryText { get; set; } = string.Empty;

        public string Days { get; set; } = string.Empty;

        public List<PLMSActivity>? ActivityList { get; set; }
    }
}
