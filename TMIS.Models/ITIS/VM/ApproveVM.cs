using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class ApproveVM
    {       
        public int AssignmentID {  get; set; }
        [DisplayName("Employee")]
        public string? EmpName {  get; set; }
        public DateTime AssignedDate { get; set; }
        [DisplayName("Assign Note")]
        public string? AssignRemarks {  get; set; }
        [DisplayName("Approver Note")]
        [Required]
        public string? ApproverRemark { get; set; }


        public int DeviceID { get; set; }

    
        [DisplayName("Asset Type")]
        public int DeviceTypeID { get; set; }
        [DisplayName("Asset Name")]
        public string? DeviceName { get; set; }

        [DisplayName("Serial #")]
        public string? SerialNumber { get; set; }

        [DisplayName("FA Code")]
        public string? FixedAssetCode { get; set; }
    
        [DisplayName("User Location")]
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

        public List<AttributeValue>? AttributeValues { get; set; }

        [DisplayName("User Department")]
        public string? AssignDepartment { get; set; }
        [DisplayName("User Location")]
        public string? AssignLocation { get; set; }
        [DisplayName("Designation")]
        public string? Designation { get; set; }
        public int AssignStatusID { get; set; }
    }
}
