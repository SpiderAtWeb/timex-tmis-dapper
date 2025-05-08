using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.PLMS
{
    public class CPathDataVM
    {
        public CPathData? CPathData { get; set; }
        public IEnumerable<SelectListItem>? InquiryTypesList { get; set; }
        public IEnumerable<SelectListItem>? ResponseTypesList { get; set; }
        public IEnumerable<SelectListItem>? CustomersList { get; set; }
        public IEnumerable<SelectListItem>? SeasonsList { get; set; }
        public IEnumerable<SelectListItem>? SampleTypesList { get; set; }
        public IEnumerable<SelectListItem>? SampleStagesList { get; set; }
        public IEnumerable<SelectListItem>? DropActivityList { get; set; }
        public IEnumerable<SelectListItem>? DropUserCategoryList { get; set; }

        public List<ActivityVM>? ActivityList { get; set; }

        public CPathDataVM()
        {
            ActivityList = []; // Initialize the list in the constructor
        }
    }
}
