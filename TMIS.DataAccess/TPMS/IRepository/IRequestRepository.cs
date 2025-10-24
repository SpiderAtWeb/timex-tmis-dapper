using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.HRRS;
using TMIS.Models.HRRS.VM;
using TMIS.Models.TPMS;
using TMIS.Models.TPMS.VM;

namespace TMIS.DataAccess.TPMS.IRepository
{
    public interface IRequestRepository
    {
        Task<bool> AddAsync(TPMS_PurchaseRequests obj);
        Task<IEnumerable<PurchaseVM>> GetAllAsync();
        Task<CreateRequestVM?> LoadListItems();
    }
}
