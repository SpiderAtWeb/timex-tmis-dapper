using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.GDRM
{
    public class GPGrUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? ErrorFieldId { get; set; } 
    }

}
