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
    public class AssignApproverVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid user.")]
        [DisplayName("User")]
        public int selectedUserID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Approver.")]
        [DisplayName("Approver")]
        public int selectedApproverID { get; set; }
        [Required]
        [DisplayName("System Type")]
        public string? selectedSystemTypeID { get; set; }
        public IEnumerable<SelectListItem>? UserList { get; set; }
        public IEnumerable<SelectListItem>? SystemTypeList { get; set; }
        public List<UserRole>? UsersRoles { get; set; }
    }
}
