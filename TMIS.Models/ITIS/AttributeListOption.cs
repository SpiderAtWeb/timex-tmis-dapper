using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class AttributeListOption
    {
        [Key]
        public int OptionID { get; set; }
        public int AttributeID {  get; set; }
        [Required]
        public string OptionText { get; set; } = string.Empty;
    }
}
