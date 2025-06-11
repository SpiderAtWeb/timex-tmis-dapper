using TMIS.Models.GDRM;

namespace TMIS.DataAccess.GDRM.IRpository
{
    public interface IGREmployee
    {
        Task<EmpGatepass?> GetEmployeeGatepassByIdAsync(int id);
        Task<EmpPendingListShow> GetEmployeePendingList();
        Task<EmpGpUpdateResult> EmployeeGatePassUpdating(EmpGpUpdate empGpUpdate);
        Task<List<EmpHistoryVM>> GetEmpHistoryData(int empGpId);
    }
}
