using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class AttributeVM
    {
        // ITIS_Attributes 
        public int AttributeID { get; set; }    
        public int DeviceTypeID { get; set; }      
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsRequired { get; set; }

        // ITIS_DeviceTypes      
        public string DeviceTypeName { get; set; } = string.Empty;
       
    }
}
