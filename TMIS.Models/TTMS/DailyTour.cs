using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class DailyTour
    {
        public int TourId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime TourDate { get; set; }

        public bool IsPresent { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation
        public string? VehicleNumber { get; set; }
        public string? EmployeeName { get; set; }
    }
}
