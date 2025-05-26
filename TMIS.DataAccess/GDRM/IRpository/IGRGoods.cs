using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;

namespace TMIS.DataAccess.GDRM.IRpository
{
    public interface IGRGoods
    {
        public Task<GPPendingListShow> GetPendingList();

        public Task<GrGatepass?> GetGatepassByIdAsync(int id, bool isOut);

        public Task<GPGrUpdateResult> GatePassUpdating(GPGrUpdate dispatch);
    }
}
