using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.SMIS;

namespace TMIS.Models.SMIS.VM
{
    public class McRequestDetailsVM
    {
        public MachinesData? oMcData { get; set; }

        public IEnumerable<SelectListItem>? LocationList { get; set; }
        public IEnumerable<SelectListItem>? UnitsList { get; set; }

        public int ReqUnitId { get; set; }
        public int ReqLocId { get; set; }
        public string? ReqRemark { get; set; }

    }
}
