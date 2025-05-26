using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.GDRM.VM
{
    public class GPPendingListShow
    {
        public int GRId { get; set; }
        public string GRName { get; set; } = string.Empty;
        public int GRLocationId { get; set; }
        public string GRLocation { get; set; } = string.Empty;
        public List<GPNumbers> GPNumbersList { get; set; } = [];
        public IEnumerable<SelectListItem> GPDriversList { get; set; } = [];
        public IEnumerable<SelectListItem> GPVehicleNoList { get; set; } = [];
    }
}
