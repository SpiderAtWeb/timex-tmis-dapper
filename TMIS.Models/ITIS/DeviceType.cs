using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class DeviceType
    {
        public int DeviceTypeID { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("Device Type")]
        public string DeviceTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Remark")]
        public string? Remarks { get; set; }
        [DisplayName("Image")]
        public byte[]? DefaultImage { get; set; }
        public bool IsDelete {  get; set; }
    }
}
