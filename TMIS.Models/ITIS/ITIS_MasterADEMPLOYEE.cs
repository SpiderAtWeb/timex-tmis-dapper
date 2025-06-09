using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class ITIS_MasterADEMPLOYEE
    {
        public int ID { get; set; }
        public int EmpNo { get; set; }
        public string? EmpName { get; set; }
        public string? EmpEmail { get; set; }
        public string? EmpDesignation { get; set; }
        public string? EmpDepartment { get; set; }
        public string? EmpLocation { get; set; }
        public bool? IsDelete { get; set; }
    }
}
