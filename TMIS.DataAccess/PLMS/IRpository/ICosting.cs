using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ICosting
    {
        Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync();

        Task<FeedbackVM> GetInquiryAsync(string id);

        Task<string> SaveCosting(int id, string costPrice, string priceComment, string fob);
    }
}
