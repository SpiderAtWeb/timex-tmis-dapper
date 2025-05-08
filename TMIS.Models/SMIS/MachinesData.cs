namespace TMIS.Models.SMIS
{
    public class MachinesData
    {
        public int Id { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public string FarCode { get; set; } = string.Empty;
        public bool IsOwned { get; set; }
        public string OwnershipStatus => IsOwned ? "Own" : "Rented";
        public DateTime? DatePurchased { get; set; }
        public DateTime? DateBorrow { get; set; }
        public DateTime? DateDue { get; set; }
        public int ServiceSeq { get; set; }
        public string MachineBrand { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;
        public string CompanyGroup { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string OwnedCluster { get; set; } = string.Empty;
        public string OwnedUnit { get; set; } = string.Empty;
        public string CurrentUnit { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string MachineModel { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string CostMethod { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public byte[]? ImageFR { get; set; }
        public byte[]? ImageBK { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}
