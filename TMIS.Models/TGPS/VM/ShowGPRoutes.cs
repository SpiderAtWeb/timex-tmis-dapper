namespace TMIS.Models.TGPS.VM
{
    public class ShowGPRoutesVM
    {
        public int GGpPassId { get; set; }
        public int Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string ROrder { get; set; } = string.Empty;
        public string RecGRName { get; set; } = string.Empty;
        public string RecUser { get; set; } = string.Empty;
        public string RecGRDateTime { get; set; } = string.Empty;
        public string RecVehicle { get; set; } = string.Empty;
        public string RecDriver { get; set; } = string.Empty;
        public string SendGRName { get; set; } = string.Empty;
        public string SendUser { get; set; } = string.Empty;
        public string SendGRDateTime { get; set; } = string.Empty;
        public string SendVehicle { get; set; } = string.Empty;
        public string SendDriver { get; set; } = string.Empty;

        public string IsReceived { get; set; } = string.Empty;
        public string IsSend { get; set; } = string.Empty;
    }
}
