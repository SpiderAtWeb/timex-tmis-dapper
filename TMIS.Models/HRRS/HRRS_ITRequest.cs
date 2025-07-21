using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.HRRS
{
    public class HRRS_ITRequest
    {
        public int RequestID { get; set; }
       
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        
        [Required]
        [Display(Name = "Employee No")]
        public string? EmployeeNo { get; set; }
        
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
        
        [Display(Name = "Replacement Of")]
        public string? Replacement { get; set; }

        [Required]
        [Display(Name = "Computer Required")]
        public string? Computer { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }

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
        public string? SIM { get; set; }

        [Display(Name = "Home Address")]        
        public string? HomeAddress { get; set; }
       
        public DateTime? RequestDate { get; set; }

        [Display(Name = "Request Remark")]
        public string? RequestRemark { get; set; }
        public DateTime? ApproverResponseDate { get; set; }
        public string? ApproverRemark { get; set; }
        public int Status { get; set; }
        public string? RequestBy { get; set; }

        //HRRS_ITReqStatus table properties
        public int id { get; set; }
        public string? PropName { get; set; }
        public string? PropDesc { get; set; }        
    }
}
