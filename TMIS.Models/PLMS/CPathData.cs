namespace TMIS.Models.PLMS
{
    public class CPathData
    {
        public int InquiryTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public int CustomerId { get; set; }
        public int SampleTypeId { get; set; }
        public int SampleStageId { get; set; }

        public int ActivityId { get; set; }
        public int UserCategoryId { get; set; }
        public int ActivityDays { get; set; }

        //public string? StyleNo { get; set; }
        //public string? StyleDesc{ get; set; }
        //public string? ColorCode { get; set; }
        //public byte[]? ImageSketch { get; set; }
        //public string? InquiryComment { get; set; }
        //public string? InquiryRecDate { get; set; }
        //public string? InquiryReqDate { get; set; }      
    }
}
