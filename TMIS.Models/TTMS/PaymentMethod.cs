using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TTMS
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string? PaymentMethodName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
