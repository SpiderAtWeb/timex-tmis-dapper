using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.GDRM
{
    public class EmpPendingListShow
    {
        public int GRId { get; set; }
        public string GRName { get; set; } = string.Empty;
        public int GRLocationId { get; set; }
        public string GRLocation { get; set; } = string.Empty;
        public List<EmpNumbers> EmpNumbersList { get; set; } = new();
        public List<SelectListItem> EmpDriversList { get; set; } = new();
        public List<SelectListItem> EmpVehicleNoList { get; set; } = new();
    }
}
