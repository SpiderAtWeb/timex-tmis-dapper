namespace TMIS.Models.TGPS
{
    public class EmployeePass
    {
        public int GuardRoomId { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int ApprovedById { get; set; }
        public string OutTime { get; set; } = string.Empty;
        public bool IsNoReturn { get; set; }
        public List<EmpPassEmp> EmpPassEmpList { get; set; } = [];

    }
}