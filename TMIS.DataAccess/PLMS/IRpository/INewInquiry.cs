using Microsoft.AspNetCore.Http;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface INewInquiry
    {
        Task<NewInquiryVM> LoadActsAndSubActsAsync(RoutePresetParas selectedInqParas);

        Task<string> SaveInquiryAsync(NewInquiryVM inquiryVM, IFormFile? artwork);

    }
}
