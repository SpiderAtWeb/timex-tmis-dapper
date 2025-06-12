using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.ITIS.VM;

namespace TMIS.Models.TAPS.VM
{
    public class UserRoleVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user.")]
        [DisplayName("User")]
        public int selectedUserID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user role.")]
        [DisplayName("User Role")]
        public int selectedUserRoleID { get; set; }
        public IEnumerable<SelectListItem>? UserRoleList { get; set; }
        public IEnumerable<SelectListItem>? UserList { get; set; }
        public List<UserRole>? UsersRoles { get; set; }
    }
}
