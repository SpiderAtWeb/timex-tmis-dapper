using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.TGPS
{
    public class AddressModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Address Name is required")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Description is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Province is required")]
        public string State { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid Contact number")]
        public string Phone { get; set; } = string.Empty;
    }
}
