using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Task<IEnumerable<SelectListItem>> GetUnitsList();

        Task SaveMachineTransferAsync(McRequestDetailsVM model);
    }
}
