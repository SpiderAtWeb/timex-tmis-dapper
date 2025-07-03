using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
    public class NextStageInquiryVM
    {
        public Inquiry? Inquiry { get; set; }     
        public IEnumerable<SelectListItem>? InquiryTypesList { get; set; }
        public IEnumerable<SelectListItem>? ResponseTypesList { get; set; }
        public IEnumerable<SelectListItem>? SampleTypesList { get; set; }
        public IEnumerable<SelectListItem>? SampleStagesList { get; set; }

        public List<ActivityVM>? ActivityList { get; set; }

        public NextStageInquiryVM()
        {
            ActivityList = []; // Initialize the list in the constructor
        }

    }
}
