using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.ITIS
{
    public class AttributeValue
    {
        public string? Name { get; set; }
        public string? ValueText { get; set; }
        public int? AttributeID {  get; set; }
        public int? AttributeType { get; set; }
    }
}
