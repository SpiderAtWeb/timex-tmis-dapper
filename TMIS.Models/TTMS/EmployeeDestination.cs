using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class EmployeeDestination
    {
        public int DestinationId { get; set; }

        [Required]
        [StringLength(200)]
        public string? DestinationName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
