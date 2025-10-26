using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class VehicleDailyCost
    {
        public int CostId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public DateTime CostDate { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
