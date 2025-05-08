using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface ITerminationRent
    {
        Task<IEnumerable<TransMC>> GetList();

        Task<MachineRentedVM?> GetRentedMcById(int id);

        Task SaveMachineObsoleteAsync(MachineRentedVM model);
    }
}
