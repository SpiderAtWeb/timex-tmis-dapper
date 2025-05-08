using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.SMIS.VM
{
    public class McCreateVM
    {
        public McInventory? McInventory { get; set; }
        public IEnumerable<SelectListItem>? MachineBrandList { get; set; }
        public IEnumerable<SelectListItem>? MachineTypesList { get; set; }
        public IEnumerable<SelectListItem>? MachineModelList { get; set; }
        public IEnumerable<SelectListItem>? GroupList { get; set; }
        public IEnumerable<SelectListItem>? LocationsList { get; set; }
        public IEnumerable<SelectListItem>? OwnedClusterList { get; set; }
        public IEnumerable<SelectListItem>? OwnedUnitList { get; set; }
    }
}
