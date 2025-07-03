using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMIS.Models.PLMS
{
    public class NewInquiryVM
    {
        public int Id { get; set; }
        public string? StyleNo { get; set; }
        public string? StyleDesc { get; set; }
        public string? ColorCode { get; set; }
        public string? Remarks { get; set; }
        public string? ReceivedDate { get; set; }
        public string? Season { get; set; }
        public bool IsPriceStageAv { get; set; }
        public bool IsSMVStageAv { get; set; }

        public int CustomerId { get; set; }
        public int InquiryTypeId { get; set; }
       
        public int SampleTypeId { get; set; }
        public int RoutingPresetsId { get; set; }

        public IEnumerable<SelectListItem>? CustomersList { get; set; }
        public IEnumerable<SelectListItem>? InquiryTypesList { get; set; }
        public IEnumerable<SelectListItem>? SampleTypesList { get; set; }
        public IEnumerable<SelectListItem>? RoutingPresetsList { get; set; }

        public List<SizeQtys>? SizeQtysList { get; set; } = [];

        [ValidateNever]
        public List<ActivityVM>? ActivityList { get; set; } = [];
    }
}
