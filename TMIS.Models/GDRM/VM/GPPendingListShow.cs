using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.GDRM.VM
{
    public class GPPendingListShow
    {
        public int GrLocRelId { get; set; }
        public int DefGrLocRelId { get; set; }
        public string GRName { get; set; } = string.Empty;

        public List<GPNumbers> GPNumbersList { get; set; } = [];
        public IEnumerable<SelectListItem> GPDriversList { get; set; } = [];
        public IEnumerable<SelectListItem> GPVehicleNoList { get; set; } = [];
    }
}
