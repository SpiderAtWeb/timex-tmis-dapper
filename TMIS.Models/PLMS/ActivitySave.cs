namespace TMIS.Models.PLMS
{
    public class ActivitySave
    {
        public int Id { get; set; }
        public string CPName { get; set; } = string.Empty;
        public List<TreeNode>? TreeData { get; set; }
    }

    public class TreeNode
    {
        public int ActivityId { get; set; }
        public int UserCategoryId { get; set; }
        public string Days { get; set; } = string.Empty;
        public bool IsAwaitTask { get; set; }
        public List<TreeNode>? ActivityList { get; set; }
    }
}
