using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.HRRS;
using TMIS.Models.HRRS.VM;

namespace TMIS.DataAccess.HRRS.IRepository
{
    public interface IITRequestRepository
    {
        Task<Create> LoadDropDowns();
        Task<bool> AddAsync(Create obj);
        void PrepairEmail(int? genId);
        Task<IEnumerable<HRRS_ITRequest>> GetAllAsync();
    }
}
