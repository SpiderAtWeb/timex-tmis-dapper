using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ISMV
    {
        Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync();

        Task<FeedbackVM> GetInquiryAsync(string id);

        Task<string> SaveSMV(int id, string smvValue, string smvComment);
    }
}
