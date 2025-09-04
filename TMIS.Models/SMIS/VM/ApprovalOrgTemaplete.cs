namespace TMIS.Models.SMIS.VM
{
    public class ApprovalOrgTemaplete  
    {
        public int Id { get; set; }
        public string ActionByName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ActionById { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string ActionOn { get; set; } = string.Empty;
        public int ActionStatus { get; set; }
    }


}
