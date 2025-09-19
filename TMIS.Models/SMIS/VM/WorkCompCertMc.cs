namespace TMIS.Models.SMIS.VM
{
    public class WorkCompCertMc
    {
        public int Id { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;    
        public string CostDisplay { get; set; } = string.Empty;
        public decimal PerDayCost { get; set; }
        public string Supplier { get; set; } = string.Empty;

    }


}
