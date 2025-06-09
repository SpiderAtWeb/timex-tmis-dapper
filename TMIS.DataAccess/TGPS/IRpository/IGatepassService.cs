using TMIS.Models.TGPS;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IGatepassService
    {
        public Task<int> GGPUpdatAsync(string gpCode, int action);

        public string GetGatepassInfoAsync(string gpCode);

        public Task<int> EGPUpdatAsync(string gpCode, int action);

        public string GetEGatepassInfoAsync(string gpCode);

    }
}
