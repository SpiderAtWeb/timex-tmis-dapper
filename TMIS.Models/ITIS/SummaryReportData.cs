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
        public int? LocationID {  get; set; }
        public int? DeviceStatusID {  get; set; }
        public string? Status {  get; set; }
        public int? DeviceTypeID {  get; set; }
        public string? DeviceType {  get; set; }
        public string? Vendor { get; set; }

    }
}
