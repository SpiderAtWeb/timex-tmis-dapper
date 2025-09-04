namespace TMIS.Models.SMIS.VM
{
    public class WorkCompCertificate
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;

        public string DeductionOfWork { get; set; } = string.Empty;
        public string DaysCount { get; set; } = string.Empty;

        public string SysTotContaractSum { get; set; } = string.Empty;

        public string InvTotContractSum { get; set; } = string.Empty;
        public string InvAdvancePayment { get; set; } = string.Empty;
        public string InvDeductPayment { get; set; } = string.Empty;
        public string InvTotalAmountPay { get; set; } = string.Empty;

        public string AmountInWords { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public ApprovalOrgTemaplete ApprovalOrg { get; set; } = new ApprovalOrgTemaplete();

        public List<WorkCompCertMc> WorkCompCertMcList { get; set; } = [];
    }


}
