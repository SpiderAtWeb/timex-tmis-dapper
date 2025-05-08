namespace TMIS.Models.SMIS
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string? QrCode { get; set; }
        public string? SerialNo { get; set; }
        public string? FarCode { get; set; }
        public string? Ownership { get; set; }
        public string? DatePurchased { get; set; }
        public string? DateBorrow { get; set; }
        public string? DateDue { get; set; }
        public int ServiceSeq { get; set; }
        public string? MachineBrand { get; set; }
        public string? MachineType { get; set; }
        public string? CompanyGroup { get; set; }
        public string? Location { get; set; }
        public string? OwnedCluster { get; set; }
        public string? OwnedUnit { get; set; }
        public string? CurrentUnit { get; set; }
        public string? MachineModel { get; set; }
        public string? Supplier { get; set; }
        public string? CostMethod { get; set; }
        public decimal Cost { get; set; }
        public string? Comments { get; set; }
        public string? Status { get; set; }


        public DateTime? LastScanDateTime { get; set; }
    }
}
