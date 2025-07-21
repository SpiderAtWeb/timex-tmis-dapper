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
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Employee No")]
        public string EmployeeNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string? Company { get; set; }

        [Required]
        [Display(Name = "NIC")]
        public string? NIC { get; set; }

        [Required]
        [Display(Name = "Interview Date")]
        [DataType(DataType.Date)]
        public DateTime? InterviewDate { get; set; }

        [Required]
        [Display(Name = "Due Start Date")]
        [DataType(DataType.Date)]
        public DateTime? DueStartDate { get; set; }

        [Required]
        [Display(Name = "Recruitment Type")]
        public string? RecruitmentType { get; set; }

        [Required]
        [Display(Name = "Computer Required")]
        public string? Computer { get; set; }

        [Required]
        [Display(Name = "Email Group")]
        public string? EmailGroup { get; set; }

        [Required]
        [Display(Name = "Computer Login")]
        public string? ComputerLogin { get; set; }

        [Required]
        [Display(Name = "SAP Access")]
        public string? SAPAccess { get; set; }

        [Required]
        [Display(Name = "WFX Access")]
        public string? WFXAccess { get; set; }

        [Required]
        [Display(Name = "New Landline")]
        public string? Landnewline { get; set; }

        [Required]
        [Display(Name = "Existing Landline")]
        public string? ExistingLandLine { get; set; }

        [Required]
        [Display(Name = "Extension")]
        public string? Extension { get; set; }

        [Required]
        [Display(Name = "Smart Phone")]
        public string? SmartPhone { get; set; }

        [Required]
        [Display(Name = "Basic Phone")]
        public string? BasicPhone { get; set; }

        [Required]
        [Display(Name = "SIM")]
        public string? SIMOnly { get; set; }

        [Required]
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }


        [Display(Name = "Request Remark")]
        public string? RequestRemark { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; } = 0; // Default status (e.g., Pending = 0)      
        public IEnumerable<SelectListItem> ChoiceList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Yes", Value = "Yes" },
            new SelectListItem { Text = "No", Value = "No" }
        };
        public List<SelectListItem> EmailGroupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "CHO_Group", Value = "CHO_Group" },
            new SelectListItem { Text = "DPDC_Group", Value = "DPDC_Group" }
        };
        public List<SelectListItem> RecruitmentTypeList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "New Recruitment", Value = "New Recruitment" },
            new SelectListItem { Text = "Replacement", Value = "Replacement" }
        };
        public List<SelectListItem> ComputerList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Text = "Desktop", Value = "Desktop" },
            new SelectListItem { Text = "Laptop", Value = "Laptop" }
        };
        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? DepartmentList { get; set; }
        public IEnumerable<SelectListItem>? DesignationList { get; set; }
        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
        public HRRS_ITRequest? HRRS_ITRequest { get; set; } = new HRRS_ITRequest();
       
    }
}
