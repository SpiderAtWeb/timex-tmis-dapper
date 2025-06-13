using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.TAPS.VM
{
    public class NewUserVM
    {
        [Required]
        [DisplayName("User")]
        public string? UserEmail { get; set; }
        [Required]
        [DisplayName("Location")]
        public string? Location { get; set; }
        [Required]
        [DisplayName("Password")]
        public string? UserPassword { get; set; }
        [Required]
        [DisplayName("User Short Name")]
        public string? UserShortName { get; set; }
        public IEnumerable<SelectListItem>? UserEmailList { get; set; }
        public IEnumerable<SelectListItem>? LocationList { get; set; }
    }
}
