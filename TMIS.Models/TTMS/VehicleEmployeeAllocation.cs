using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class VehicleEmployeeAllocation
    {
        public int AllocationId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime AllocationDate { get; set; }

        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public string? VehicleNumber { get; set; }
        public string? EmployeeName { get; set; }
    }
}
