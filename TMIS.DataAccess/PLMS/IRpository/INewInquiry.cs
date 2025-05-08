using Microsoft.AspNetCore.Http;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface INewInquiry
    {
        Task<InquiryVM> LoadActsAndSubActsAsync(InquiryParams selectedInqParas);

        Task<string> SaveMasterInquiryAsync(InquiryVM inquiryVM, IFormFile? artwork);

    }
}
