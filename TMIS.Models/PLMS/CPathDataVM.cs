using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.PLMS
{
    public class CPathDataVM
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int UserCategoryId { get; set; }
        public int ActivityDays { get; set; }

        public IEnumerable<SelectListItem>? CPathList { get; set; }
        public IEnumerable<SelectListItem>? DropActivityList { get; set; }
        public IEnumerable<SelectListItem>? DropUserCategoryList { get; set; }

        public List<ActivityVM>? ActivityList { get; set; }

        public CPathDataVM()
        {
            ActivityList = [];
        }
    }
}
