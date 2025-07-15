using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.HRRS
{
    public class HRRS_ITRequest
    {
        public int ID { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeNo { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Company { get; set; }
        public string? NIC { get; set; }
        public DateTime? InterviewDate { get; set; }
        public DateTime? DueStartDate { get; set; }
        public string? RecruitmentType { get; set; }
        public string? Computer { get; set; }
        public string? ComputerGroup { get; set; }
        public string? ComputerLogin { get; set; }
        public string? SAPAccess { get; set; }
        public string? Landnewline { get; set; }
        public string? ExistingLandLine { get; set; }
        public string? Extension { get; set; }
        public string? SmartPhone { get; set; }
        public string? BasicPhone { get; set; }
        public string? SMSOnly { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? RequestRemark { get; set; }
        public DateTime? ApproverResponseDate { get; set; }
        public string? ApproverRemark { get; set; }
        public int Status { get; set; }
    }
}
