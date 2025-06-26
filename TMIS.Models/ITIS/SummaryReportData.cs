using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class SummaryReportData
    {
        public int DeviceID {  get; set; }
        public string? SerialNumber {  get; set; }
        public string? FixedAssetCode {  get; set; }
        public string? DeviceName {  get; set; }
        public string? PurchasedDate {  get; set; }
        public string? LocationName {  get; set; }
        public string? LocationID {  get; set; }
        public int? DeviceStatusID {  get; set; }
        public string? Status {  get; set; }
        public int? DeviceTypeID {  get; set; }
        public string? DeviceType {  get; set; }
        public string? Vendor { get; set; }
        public string? EMPNo { get; set; }
        public string? EmpEmail { get; set; }
        public string? EmpName { get; set; }
        public string? Designation { get; set; }
        public string? AssignedDate { get; set; }
        public string? ReturnedDate { get; set; }
        public string? ReturnRemarks { get; set; }
        public string? AssignRemarks { get; set; }
        public string? currentStatus { get; set; }
        public string? UserStatus { get; set; }

    }
}
