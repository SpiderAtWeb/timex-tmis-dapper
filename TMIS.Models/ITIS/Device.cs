using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Asset Type")]
        public int DeviceTypeID { get; set; }        
        [DisplayName("Asset Name")]
        public string? DeviceName { get; set; }
        [Required]
        [DisplayName("Serial #")]
        public string? SerialNumber { get; set; }
        
        [DisplayName("FA Code")]
        public string? FixedAssetCode { get; set; }
        [Required]
        [DisplayName("Location")]
        public string? Location { get; set; }
       
        [DisplayName("Image 1")]
        public byte[]? Image1Data { get; set; }
        [DisplayName("Image 2")]
        public byte[]? Image2Data { get; set; }
        [DisplayName("Image 3")]
        public byte[]? Image3Data { get; set; }
        [DisplayName("Image 4")]
        public byte[]? Image4Data { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Purchased Date")]
        public DateTime? PurchasedDate { get; set; }

        public string? AddedBy { get; set; }

        public DateTime? AddedDate { get; set; }

        [Required]
        [DisplayName("Asset is currently")]
        public int DeviceStatusID { get; set; }

        public DateTime? UpdatedOn { get; set; }
        [DisplayName("Description")]
        public string? Remark { get; set; }
        [DisplayName("Depreciation")]
        public string? Depreciation { get; set; }
        [DisplayName("Vendor")]
        public int? VendorID { get; set; }
        [DisplayName("Is Rented")]
        public bool IsRented { get; set; }
        [DisplayName("Is Brand New")]
        public bool IsBrandNew { get; set; }
        public string? Status {  get; set; }
        public string? Vendor {  get; set; }
        public string? DeviceType {  get; set; }

        public List<AttributeValue>? AttributeValues { get; set; }
        [Required]
        [DisplayName("Cost Center")]
        public string? Department { get; set; }

        //Assignment Details
        public string? EmpName { get; set; }
    }
}
