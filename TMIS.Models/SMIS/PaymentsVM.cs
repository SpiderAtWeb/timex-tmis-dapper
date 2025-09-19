namespace TMIS.Models.SMIS
{
    public class PaymentsVM
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string RaisedDate { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string VendorTotalAmount { get; set; } = string.Empty;

    }
}
