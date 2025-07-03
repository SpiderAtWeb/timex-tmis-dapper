using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ITaskCompletion
    {
        Task<string> SaveTasksAndSubTasksAsync(SaveTasks saveTasks);

        Task<IEnumerable<ShowInquiryDataVM>> GetInquiriesUserIdAsync();

        Task<ModalShowVM> LoadModalDataUserIdAsync(string Id);
    }
}
