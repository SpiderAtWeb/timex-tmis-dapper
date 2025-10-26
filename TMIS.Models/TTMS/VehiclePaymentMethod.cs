using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class VehiclePaymentMethod
    {
        public int VehiclePaymentId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public string? VehicleNumber { get; set; }
        public string? PaymentMethodName { get; set; }
    }
}
