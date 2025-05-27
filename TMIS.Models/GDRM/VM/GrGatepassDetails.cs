using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.GDRM.VM
{
    public class GrGatepassDetails
    {
        public int ID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemDesc { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string GpUnits { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> GrDispReasonList { get; set; } = [];

    }
}
