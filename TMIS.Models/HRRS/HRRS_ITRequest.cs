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
        //HRRS_ITRequests table properties
        [Key]
        public int RequestID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "Employee No")]
        [StringLength(20)]
        public string? EmployeeNo { get; set; }

        [Required]
        [Display(Name = "Designation")]
        [StringLength(50)]
        public string? Designation { get; set; }

        [Required]
        [Display(Name = "Department")]
        [StringLength(50)]
        public string? Department { get; set; }

        [Required]
        [Display(Name = "Location")]
        [StringLength(50)]
        public string? Location { get; set; }

        [Required]
        [Display(Name = "Company")]
        [StringLength(50)]
        public string? Company { get; set; }

        [Required]
        [Display(Name = "NIC")]
        [StringLength(50)]
        public string? NIC { get; set; }

        [Required]
        [Display(Name = "Interview Date")]
        [DataType(DataType.Date)]
        public DateTime? InterviewDate { get; set; }

        [Required]
        [Display(Name = "Due Start Date")]
        [DataType(DataType.Date)]
        public DateTime? DueStartDate { get; set; }

        [Display(Name = "Recruitment Type")]
        public bool IsReplacement { get; set; } = false;

        [Display(Name = "Replacement Of")]
        [StringLength(100)]
        public string? Replacement { get; set; }

        [Display(Name = "Email")]
        public bool Email { get; set; } = false;

        [Display(Name = "Email Group")]
        [StringLength(50)]
        public string? EmailGroup { get; set; }

        [Display(Name = "Computer Required")]
        [StringLength(50)]
        public string? Computer { get; set; }
        
        [Display(Name = "Computer Login")]
        public bool ComputerLogin { get; set; } = false;

        [Display(Name = "SAP Access")]
        public bool SAP { get; set; } = false;

        [Display(Name = "WFX Access")]
        public bool WFX { get; set; } = false;

        [Display(Name = "New Landline")]
        public bool NewLandline { get; set; } = false;

        [Display(Name = "Existing Landline")]
        public bool ExistingLandline { get; set; } = false;

        [Display(Name = "Extension")]
        public bool Extension { get; set; } = false;

        [Display(Name = "Smart Phone")]
        public bool SmartPhone { get; set; } = false;

        [Display(Name = "Basic Phone")]
        public bool BasicPhone { get; set; } = false;

        [Display(Name = "SIM")]
        public bool SIM { get; set; } = false;

        [Display(Name = "Home Address")]
        [StringLength(500)]
        public string? HomeAddress { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Request Remark")]
        [StringLength(500)]
        public string? RequestRemark { get; set; }

        public DateTime? ApproverResponseDate { get; set; }

        [StringLength(500)]
        public string? ApproverRemark { get; set; }

        public int Status { get; set; }

        [StringLength(50)]
        public string? RequestBy { get; set; }

        public bool IsDelete { get; set; } = false;


        //HRRS_ITReqStatus table properties
        public int id { get; set; }
        public string? PropName { get; set; }
        public string? PropDesc { get; set; }        
    }
}
