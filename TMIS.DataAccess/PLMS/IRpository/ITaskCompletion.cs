using TMIS.Models.PLMS;

namespace TMIS.DataAccess.PLMS.IRpository
{
    public interface ITaskCompletion
    {
        Task<IEnumerable<ShowInquiryDataVM>> GetTasksListAsync();

        Task<string> SaveTasksAndSubTasksAsync(SaveTasks saveTasks);
    }
}
