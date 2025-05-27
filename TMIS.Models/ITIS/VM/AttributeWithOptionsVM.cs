using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS.VM
{
    public class AttributeWithOptionsVM
    {
        public int AttributeID { get; set; }
        public string? Name { get; set; }
        public int DataType { get; set; }
        public List<string>? Options { get; set; }

        public string? Value {  get; set; } 
    }
}
