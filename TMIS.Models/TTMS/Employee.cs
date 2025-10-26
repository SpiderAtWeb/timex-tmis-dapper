using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? EmployeeCode { get; set; }

        [Required]
        [StringLength(200)]
        public string? EmployeeName { get; set; }

        public int? DestinationId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public string? DestinationName { get; set; }
    }
}
