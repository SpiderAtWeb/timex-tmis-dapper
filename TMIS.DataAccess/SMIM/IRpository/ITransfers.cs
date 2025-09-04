using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface ITransfers
    {
        Task<IEnumerable<TransMC>> GetList();

        Task<IEnumerable<TransMCUser>> GetListUser(string pDate);

        Task<MachinesData> GetMachineData(int pMcId);

        Task<IEnumerable<SelectListItem>> GetLocationsList();

        Task SaveMachineTransferAsync(McRequestDetailsVM model);
    }
}
