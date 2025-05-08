using TMIS.Models.PLMS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface IOverview
    {
        Task<IEnumerable<PendingActivity>> GetAllRunningInqsData();
    }
}
