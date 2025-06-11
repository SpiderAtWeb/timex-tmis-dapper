using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.PLMS
{
    public class InquiryVM
    {
        public Inquiry? Inquiry { get; set; }
        public IEnumerable<SelectListItem>? InquiryTypesList { get; set; }
        public IEnumerable<SelectListItem>? ResponseTypesList { get; set; }
        public IEnumerable<SelectListItem>? CustomersList { get; set; }
        public IEnumerable<SelectListItem>? SeasonsList { get; set; }
        public IEnumerable<SelectListItem>? SampleTypesList { get; set; }
        public IEnumerable<SelectListItem>? SampleStagesList { get; set; }

        [ValidateNever]
        public List<ActivityVM>? ActivityList { get; set; }

        public InquiryVM()
        {
            ActivityList = [];
        }
    }
}
