using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.TGPS;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IResponse
    {
        public Task<IEnumerable<GoodsPassList>> GetList();

        public Task<ShowGPListVM?> LoadShowGPDataAsync(int id);

        public Task<bool> HandleGGpAction(int id, string action);


        public Task<IEnumerable<EmpPassVM>> GetEmpList();

        public Task<EmpPassVM> GetEmpPassesAsync(int id);

        public Task<bool> HandleEGpAction(int id, string action);
    }
}
