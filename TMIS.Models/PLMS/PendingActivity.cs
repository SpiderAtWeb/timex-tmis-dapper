using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
    public class PendingActivity
    {
        public int Id { get; set; }
        public string InquiryRef { get; set; }  =   string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string StyleNo { get; set; } = string.Empty;
        public string StyleDesc { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string InquiryType { get; set; } = string.Empty;
        public string ResponseType { get; set; } = string.Empty;
        public string Seasons { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string SampleStage { get; set; } = string.Empty;
        public string InquiryRecDate { get; set; } = string.Empty;
        public string DateResponseReq { get; set; } = string.Empty;
        public string InquiryComment { get; set; } = string.Empty;
        public string IsFOBOrCM { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SMV { get; set; }


        public string BuyerComments { get; set; } = string.Empty;
        public string DateActualRespRec { get; set; } = string.Empty;
    }
}
