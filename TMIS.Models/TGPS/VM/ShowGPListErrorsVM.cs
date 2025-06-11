namespace TMIS.Models.TGPS.VM
{
    public class ShowGPListErrorsVM
    {
        public int GpRouteId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SendError { get; set; } = string.Empty;
        public string RecError { get; set; } = string.Empty;
        public decimal SendQty { get; set; }
        public decimal RecQty { get; set; }
    }
}
