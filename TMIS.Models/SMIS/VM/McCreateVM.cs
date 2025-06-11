using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.SMIS.VM
{
    public class McCreateVM
    {
        public McInventory? McInventory { get; set; }
        public IEnumerable<SelectListItem>? BrandsList { get; set; }
        public IEnumerable<SelectListItem>? TypesList { get; set; }
        public IEnumerable<SelectListItem>? ModelsList { get; set; }    
        public IEnumerable<SelectListItem>? OwnedLocList { get; set; }
        public IEnumerable<SelectListItem>? LinesList { get; set; }
    }
}
