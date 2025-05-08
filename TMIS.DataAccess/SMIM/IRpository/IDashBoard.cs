using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IDashBoard
    {
        Task<List<MachineStatus>> GetDashBoardData();

        Task<IEnumerable<DashboardSummary>> GetSmryDataAsync(string clusterId);

        string[] GetTrLoggerData(string mcId);

        Task<IEnumerable<InventoryItem>> GetAllInventoryData();

        Task<IEnumerable<PivotDataVM>> GetPivotData(string clusterId);

        Task<IEnumerable<SelectListItem>> GetClusterDetails();


    }
}
