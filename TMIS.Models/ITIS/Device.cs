using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class Device
    {
        [Key]
        public int DeviceID { get; set; }

        [Required]
        public int DeviceTypeID { get; set; }

        public string? DeviceName { get; set; }

        public string? SerialNumber { get; set; }

        public string? FixedAssetCode { get; set; }

        public string? Location { get; set; }

        public byte[] Image1Data { get; set; }

        public byte[] Image2Data { get; set; }

        public byte[] Image3Data { get; set; }

        public byte[] Image4Data { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PurchasedDate { get; set; }

        public string? AddedBy { get; set; }

        public DateTime? AddedDate { get; set; }

        [Required]
        public int DeviceStatusID { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? Remark { get; set; }

        public string? Depreciation { get; set; }

        public int? VendorID { get; set; }

        public bool? IsRented { get; set; }

        public bool? IsBrandNew { get; set; }
    }
}
