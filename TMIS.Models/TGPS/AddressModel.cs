using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TGPS
{
    public class AddressModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Address Name is required")]
        public string AddressName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Lane 1 is required")]
        public string AddressAddressLane1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Lane 2 is required")]
        public string AddressAddressLane2 { get; set; } = string.Empty;

          [Required(ErrorMessage = "Address Lane 3 is required")]
        public string AddressAddressLane3 { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactNos { get; set; } = string.Empty;
    }
}
