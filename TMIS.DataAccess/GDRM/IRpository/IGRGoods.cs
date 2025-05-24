using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;

namespace TMIS.DataAccess.GDRM.IRpository
{
    public interface IGRGoods
    {
        public Task<GPDispatchingVM> GetDispachingList();

        public Task<GrGatepass?> GetGatepassByIdAsync(int id);

        public Task<bool> DispatchingGoods(Dispatching dispatch);
    }
}
