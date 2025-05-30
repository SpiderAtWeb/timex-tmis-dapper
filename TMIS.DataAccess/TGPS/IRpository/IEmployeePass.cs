using TMIS.Models.TGPS;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IEmployeePass
    {
        public Task<EmployeePassVM> GetAllAsync();

        public Task<int> InsertEmployeePassAsync(EmployeePassVM model);
    }
}
