using TMIS.Models.TGPS;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IAddressBank
    {
        Task<IEnumerable<AddressModel>> GetAllAsync();
        Task<AddressModel?> GetByIdAsync(int id);
        Task<int> InsertAsync(AddressModel model);
        Task<int> UpdateAsync(AddressModel model);
        Task<int> DeleteAsync(int id);

    }
}
