using System.Text.Json.Serialization;

namespace TMIS.Models.PLMS
{
    public class InquiryParams
    {
       public int InquiryTypeId { get; set; }

        public int ResponseTypeId { get; set; }

        public int CustomerId { get; set; }

        public int SampleTypeId { get; set; }

        public int SampleStageId { get; set; }

        public string SelectedDate { get; set; } = string.Empty;
    }
}
