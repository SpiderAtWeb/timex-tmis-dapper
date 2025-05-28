using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TMIS.Models.ITIS
{
    public class AttributeModel
    {
        public int AttributeID {  get; set; }
        [Required]
        [DisplayName("Device Type")]
        public string? DeviceTypeID { get; set; }
        [Required]
        [DisplayName("Label")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [DisplayName("Attribute Type")]
        public string  DataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
    }
}
