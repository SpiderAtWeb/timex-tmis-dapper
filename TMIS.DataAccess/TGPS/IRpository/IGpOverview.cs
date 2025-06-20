using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IGpOverview
    {
        public Task<IEnumerable<GoodsPassList>> GetList();
    }
}
