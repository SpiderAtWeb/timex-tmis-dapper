using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
   public class ShowInquiryDataVM
    {
        public string Id { get; set; } = string.Empty;
        public string InquiryRef { get; set; } = string.Empty;
        public string StyleNo { get; set; } = string.Empty;
        public string StyleDesc { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string InquiryType { get; set; } = string.Empty;
        public string ResposeType { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Seasons { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string SampleStage { get; set; } = string.Empty;
        public string InquiryComment { get; set; } = string.Empty;
    }
}
