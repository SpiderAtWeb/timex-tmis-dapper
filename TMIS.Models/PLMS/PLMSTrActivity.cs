using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
    public class PLMSTrActivity
    {
        public int TaskId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityText { get; set; } = string.Empty;
        public string SubActivityText { get; set; } = string.Empty;
        public string ActivityRequiredDate { get; set; } = string.Empty;

        public string ActivityComment { get; set; } = string.Empty;
        public string ActivityActualCmpltdDate { get; set; } = string.Empty;
        public string ActivityDoneComment { get; set; } = string.Empty;
        public string ActivityDoneBy { get; set; } = string.Empty;
        public int DueDates { get; set; }


        public bool ActivityIsCompleted { get; set; }

        public List<PLMSTrActivity>? SubActivityList { get; set; }
    }
}
