using TMIS.Models.TGPS;
using TMIS.Models.TGPS.VM;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IEmployeePass
    {
        public Task<IEnumerable<EmpPassVM>> GetList();
        public Task<EmployeePassVM> GetAllAsync();
        public Task<string> InsertEmployeePassAsync(EmployeePassVM model);
        public Task<EmpPassVM> GetEmpPassesAsync(int id);
    }
}
