using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TGPS
{
    public class EmployeePass
    {

        [Required(ErrorMessage = "Guard room cannot be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Guard room cannot be empty")]
        public int GuardRoomId { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Approve Person cannot be empty")]
        [Range(1, int.MaxValue, ErrorMessage = "Approve Person cannot be empty")]
        public int ApprovedById { get; set; }

        [Required]
        public string OutTime { get; set; } = string.Empty;

        public bool IsNoReturn { get; set; }

        [Required]
        public List<EmpPassEmp> EmpPassEmpList { get; set; } = [];

    }
}