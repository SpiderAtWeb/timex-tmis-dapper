using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.GDRM
{
    public class EmpGatepassDetails
    {
        public int Id { get; set; }
        public string EmpName { get; set; } = string.Empty;
        public string EmpEPF { get; set; } = string.Empty;
        public DateTime? ActualTime { get; set; }
        public string ResponsedUser { get; set; } = string.Empty;
    }
}
