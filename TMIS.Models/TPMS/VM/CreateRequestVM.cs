using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TPMS.VM
{
    public class CreateRequestVM
    {
        public IEnumerable<SelectListItem>? SerialNumberList { get; set; }
        public IEnumerable<SelectListItem>? DepartmentList { get; set; }
        public IEnumerable<SelectListItem>? UnitList { get; set; }
        public IEnumerable<SelectListItem>? DesignationList { get; set; }
        public IEnumerable<SelectListItem>? UserList { get; set; }
        public TPMS_PurchaseRequests? TPMS_PurchaseRequests { get; set; }
        public bool IsNew { get; set; } = false;

    }
}
