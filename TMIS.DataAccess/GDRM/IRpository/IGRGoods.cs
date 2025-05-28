using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.GDRM.IRpository
{
    public interface IGRGoods
    {
        public Task<GPPendingListShow> GetPendingList();

        public Task<GrGatepass?> GetGatepassByIdAsync(int id);

        public Task<GPGrUpdateResult> GatePassUpdating(GPGrUpdate dispatch);

        public Task<List<GpHistoryVM>> GetGDHistoryData(int id);
    }
}
