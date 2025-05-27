using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TGPS.VM
{
    public class GoodPassVM
    {
        public int Id { get; set; }
        public IEnumerable<SelectListItem>? GoodsFrom { get; set; }
        public IEnumerable<SelectListItem>? GoodsTo { get; set; }
        public IEnumerable<SelectListItem>? ApprovalList { get; set; }
        public IEnumerable<SelectListItem>? Units { get; set; }
    }
}
