using Microsoft.AspNetCore.Http;
using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface INextStages
    {
        Task<NextStageInquiryVM> LoadNextInquiryDropDowns(string id);
        Task<string> SaveNextInquiryAsync(NextStageInquiryVM inquiryVM, IFormFile? artwork);
    }
}
