using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.TAPS;

namespace TMIS.DataAccess.TAPS.IRepository
{
    public interface IAdminRepository
    {
        Task<IEnumerable<SelectListItem>> LoadUserRoles();
        Task<IEnumerable<SelectListItem>> LoadUsers();
        Task<IEnumerable<UserRole>> LoadUserRole(int UserId);
        Task<bool> AssignUserRole(int userID, int roleID);
        Task<bool> CheckRoleExistToUser(int userID, int roleID);
        void DeleteUserRole(int userID, int roleID);
    }
}
