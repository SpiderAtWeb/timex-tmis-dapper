namespace TMIS.Models.PLMS
{
    public class ActivitySave
    {
        public int HeaderId { get; set; }
        public int SelectedInqTypeId { get; set; }
        public int SelectedRepTypeId { get; set; }
        public int SelectedCustomerId { get; set; }
        public int SelectedSampTypeId { get; set; }
        public int SelectedSampStageId { get; set; }

        public List<TreeNode>? TreeData { get; set; }
    }

    public class TreeNode
    {
        public int ActivityId { get; set; }
        public int UserCategoryId { get; set; }
        public string Days { get; set; } = string.Empty;
        public List<TreeNode>? ActivityList { get; set; }
    }
}
