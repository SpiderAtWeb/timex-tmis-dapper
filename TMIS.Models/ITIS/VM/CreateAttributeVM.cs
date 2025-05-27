using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.ITIS.VM
{
    public class CreateAttributeVM
    {
        public AttributeModel? Attribute { get; set; }
        public List<AttributeListOption>? AttributeListOption { get; set; }      
        public IEnumerable<SelectListItem>? DeviceTypeList { get; set; }
        public IEnumerable<SelectListItem>? AttributeTypeList { get; set; }        
    }
}
