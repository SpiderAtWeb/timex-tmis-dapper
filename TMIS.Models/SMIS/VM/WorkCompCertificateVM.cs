namespace TMIS.Models.SMIS.VM
{
    public class WorkCompCertificateVM
    {
        //Collect
        public int Id { get; set; } 
        public string RaisedDate { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;      
        public string InvoiceFromDate { get; set; } = string.Empty;
        public string InvoiceToDate { get; set; } = string.Empty;
        public int DaysCount { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string CertificateRemarks { get; set; } = string.Empty;
        public string SystemCalculatedSum { get; set; } = string.Empty;
        public string VendorContractSum { get; set; } = string.Empty;
        public string VendorAdvancePayment { get; set; } = string.Empty;
        public string VendorTotalAmount { get; set; } = string.Empty;

        public string PreparedBy { get; set; } = string.Empty;
        public string GeneratedOn { get; set; } = string.Empty;

        public string ApproveLevel2By { get; set; } = string.Empty;
        public string AppLevelStat2 { get; set; } = string.Empty;
        public string AppLevelStat2On { get; set; } = string.Empty;

        public string ApproveLevel3By { get; set; } = string.Empty;
        public string AppLevelStat3 { get; set; } = string.Empty;
        public string AppLevelStat3On { get; set; } = string.Empty;

        public string ApproveLevel4By { get; set; } = string.Empty;
        public string AppLevelStat4 { get; set; } = string.Empty;
        public string AppLevelStat4On { get; set; } = string.Empty;

        public string ApproveLevel5By { get; set; } = string.Empty;
        public string AppLevelStat5 { get; set; } = string.Empty;
        public string AppLevelStat5On { get; set; } = string.Empty;

        public string ApproveLevel6By { get; set; } = string.Empty;
        public string AppLevelStat6 { get; set; } = string.Empty;
        public string AppLevelStat6On { get; set; } = string.Empty;
        public bool TaskComplete { get; set; }
        public bool TaskStart { get; set; }
        public string ProcessStartDate { get; set; } = string.Empty;


        public List<WorkCompCertMc> WorkCompCertMcList { get; set; } = [];

    }


}
