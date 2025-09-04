using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IRenting
    {
        Task<IEnumerable<TransMC>> GetList();

        Task<IEnumerable<TransMC>> GetListPayments();

        Task<MachineRentedVM?> GetRentedMcById(int id);

        Task<string[]> UpdateStatus(string remarks, string id, bool action);

        Task<WorkCompCertificate?> GetMachinesByIdsAsync(List<int> ids);
    }
}
