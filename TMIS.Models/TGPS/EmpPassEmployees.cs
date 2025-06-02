namespace TMIS.Models.TGPS
{
    public class EmpPassEmployees
    {
        public int Id { get; set; }
        public string EGpPassId { get; set; } = string.Empty;
        public string EmpName { get; set; } = string.Empty;
        public string EmpEPF { get; set; } = string.Empty;
        public string ActualOutTime { get; set; } = string.Empty;
        public string ActualInTime { get; set; } = string.Empty;
    }
}
