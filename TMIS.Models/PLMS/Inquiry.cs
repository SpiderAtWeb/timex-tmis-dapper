namespace TMIS.Models.PLMS
{
    public class Inquiry
    {
        public int Id { get; set; }
        public int TrINQId { get; set; }
        public int InquiryTypeId { get; set; }
        public string InquiryType { get; set; } = string.Empty;
        public int ResponseTypeId { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public int SampleTypeId { get; set; }
        public int SampleStageId { get; set; }
        public bool IsPriceStageAv { get; set; }
        public bool IsSMVStageAv { get; set; }

        public string? StyleNo { get; set; }
        public string? StyleDesc{ get; set; }
        public string? ColorCode { get; set; }
        public string? InquiryComment { get; set; }
        public string? InquiryRecDate { get; set; }
        public string? InquiryReqDate { get; set; }

        public int RoutingPresetsId { get; set; }
    }
}
