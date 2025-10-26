using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class TransportVehicle
    {
        public int VehicleId { get; set; }

        [Required]
        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        [StringLength(200)]
        public string? VehicleName { get; set; }

        public int? DestinationId { get; set; }
        public int? Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public string? DestinationName { get; set; }
    }
}
