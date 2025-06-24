namespace TMIS.Models.PLMS
{
    public class ActivityVM
    {
        public int ActivityId { get; set; }
        public string ActiviytName { get; set; } = string.Empty;
        public string ActiviytValue { get; set; } = string.Empty;
        public bool IsAwaitingTask { get; set; }
        public string ActivityComment { get; set; } = string.Empty;
        public string UserCategoryId { get; set; } = string.Empty;
        public string UserCategoryText { get; set; } = string.Empty;

        public List<ActivityVM>? SubActivityList { get; set; }

    }
}
