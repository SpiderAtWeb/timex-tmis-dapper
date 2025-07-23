using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.HRRS.VM
{
    public class ITRequestTable
    {
        public int? ID { get; set; }
        public string? TblFirstName { get; set; }
        public string? TblLastName { get; set; }
        public string? TblEmployeeNo { get; set; }
        public string? TblDesignation { get; set; }
        public string? TblDepartment { get; set; }
        public string? TblLocation { get; set; }
        public string? TblCompany { get; set; }
        public string? TblNIC { get; set; }
        public DateTime? TblDueStartDate { get; set; }
        public string? TblRecruitmentType { get; set; }
        public string? TblStatus { get; set; }
    }
}
