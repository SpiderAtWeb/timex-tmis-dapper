using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.HRRS.VM
{
    public class Create
    {
        public IEnumerable<SelectListItem> ChoiceList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Yes", Value = "Yes" },
            new SelectListItem { Text = "No", Value = "No" }
        };
        public List<SelectListItem> EmailGroupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "CHO_Group", Value = "CHO_Group" },
            new SelectListItem { Text = "DPDC_Group", Value = "DPDC_Group" },
            new SelectListItem { Text = "TIMEX_ALL_USERS", Value = "TIMEX_ALL_USERS" },
        };
        public List<SelectListItem> RecruitmentTypeList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "New Recruitment", Value = "New Recruitment" },
            new SelectListItem { Text = "Replacement", Value = "Replacement" }
        };
        public List<SelectListItem> ComputerList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Desktop", Value = "Desktop" },
            new SelectListItem { Text = "Laptop", Value = "Laptop" },
        };
        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? DepartmentList { get; set; }
        public IEnumerable<SelectListItem>? DesignationList { get; set; }
        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
        public HRRS_ITRequest? HRRS_ITRequest { get; set; } = new HRRS_ITRequest();

    }
}
