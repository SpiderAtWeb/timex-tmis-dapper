using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ICommon
    {
        Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesAsync();

        Task<ModalShowVM> LoadModalDataAsync(string Id);

        Task<NewInquiryVM> LoadInquiryDropDowns();
    }
}
