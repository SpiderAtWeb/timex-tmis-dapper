using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.TAPS.VM
{
    public class UserLocationVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user.")]
        [DisplayName("User")]
        public int selectedUserID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid location.")]
        [DisplayName("Location")]
        public int selectedlocationID { get; set; }
        public IEnumerable<SelectListItem>? UserLocationList { get; set; }
        public IEnumerable<SelectListItem>? UserList { get; set; }
        public List<UserLocation>? UserLocations { get; set; }
    }
}
