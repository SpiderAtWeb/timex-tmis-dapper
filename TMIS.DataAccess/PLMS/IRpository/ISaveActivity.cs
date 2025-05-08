using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ISaveCriticalPathActivity
    {
        public Task SaveActivities(ActivitySave activitySave);

        public Task<CPathDataVM> LoadCPathDropDowns();

        Task<List<PLMSActivity>> LoadSavedActivityList(InquiryParams inqParas);

    }
}
