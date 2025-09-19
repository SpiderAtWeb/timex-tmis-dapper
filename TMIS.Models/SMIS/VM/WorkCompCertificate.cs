using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.SMIS.VM
{
    public class WorkCompCertificate
    {
        //Collect
       
        public IEnumerable<SelectListItem>? UnitsList { get; set; }
        public int UnitId { get; set; }
        public int ApprovalCatId { get; set; }
        public IEnumerable<SelectListItem>? ApprovalCat { get; set; }
        public IEnumerable<SelectListItem>? SupplierList { get; set; }
        public int SupplierId { get; set; }
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string InvContractSum { get; set; } = string.Empty;
        public string InvAdvancePayment { get; set; } = string.Empty;
        public string InvTotalAmountPay { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public List<WorkCompCertMc> WorkCompCertMcList { get; set; } = [];

        public int FactoryAcctId { get; set; }
        public int GeneralMgrId { get; set; }
        public int GroupEngId { get; set; }
        public int AppLevel1Id { get; set; }
        public int AppLevel2Id { get; set; }        

    }


}
