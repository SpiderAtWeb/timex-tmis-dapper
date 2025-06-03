using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IGoodsGatePass
    {
        public Task<IEnumerable<GoodsPassList>> GetList();
        public Task<GoodPassVM> GetSelectData();
        public Task<string> GenerateGatePass(GatepassVM model);
        public Task<List<GpHistoryVM>> GetHistoryData(int gpId);

        public Task<ShowGPListVM?> LoadShowGPDataAsync(int id);
    }
}
