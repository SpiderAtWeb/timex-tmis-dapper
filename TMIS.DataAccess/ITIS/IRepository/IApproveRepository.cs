using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IApproveRepository
    {
        Task<IEnumerable<ApproveDeviceUserVM>> GetAllAsync();
        Task <ApproveVM?> GetSelectedRecord(int assignmentID);
        Task <bool> AddAsync(ApproveVM obj);
        Task <bool> Reject(ApproveVM obj);

    }
}
