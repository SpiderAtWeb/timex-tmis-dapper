using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        [DisplayName("Employee Code")]
        [Required]
        public string EmployeeCode { get; set; } = string.Empty;
        [DisplayName("Employee Name")]
        [Required]
        public string EmployeeName { get; set; } = string.Empty;
        [DisplayName("Route")]
        [Required]
        public int DestinationId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        [DisplayName("Location")]
        [Required]
        public int LocationId { get; set; }
    }
}
