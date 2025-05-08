using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.SMIS;

namespace TMIS.Models.SMIS.VM
{
    public class McCreatedRnVM
    {
        public McInventory? McInventory { get; set; }

        public IEnumerable<SelectListItem>? MachineBrandList { get; set; }
        public IEnumerable<SelectListItem>? MachineTypesList { get; set; }
        public IEnumerable<SelectListItem>? MachineModelList { get; set; }
        public IEnumerable<SelectListItem>? GroupList { get; set; }
        public IEnumerable<SelectListItem>? LocationsList { get; set; }
        public IEnumerable<SelectListItem>? OwnedClusterList { get; set; }
        public IEnumerable<SelectListItem>? OwnedUnitList { get; set; }
        public IEnumerable<SelectListItem>? SupplierList { get; set; }
        public IEnumerable<SelectListItem>? CostMethodsList { get; set; }

    }
}
