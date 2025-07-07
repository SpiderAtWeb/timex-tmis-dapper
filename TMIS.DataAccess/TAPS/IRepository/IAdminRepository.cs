using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.Models.TAPS;
using TMIS.Models.TAPS.VM;

namespace TMIS.DataAccess.TAPS.IRepository
{
    public interface IAdminRepository
    {
        Task<IEnumerable<SelectListItem>> LoadUserRoles();
        Task<IEnumerable<SelectListItem>> LoadUsers();
        Task<IEnumerable<UserRole>> LoadUserRole(int UserId);
        Task<bool> AssignUserRole(int userID, int roleID);
        Task<bool> CheckRoleExistToUser(int userID, int roleID);
        Task<bool> CheckLocationExistToUser(int userID, int roleID);
        void DeleteUserRole(int userID, int roleID);
        Task<IEnumerable<SelectListItem>> LoadEmployeeList();
        Task<bool> InsertNewUser(NewUserVM newUserVM);
        Task<bool> CheckUserEmailExist(string userEmail);
        Task<IEnumerable<SelectListItem>> LoadLocationList();
        Task<bool> CheckApproverExistToUser(AssignApproverVM obj);
        Task<bool> InsertApprover(AssignApproverVM obj);
        void DeleteApprover(AssignApproverVM obj);
        Task<IEnumerable<UserApprover>> LoadUserApprovers(int userID);
        Task<IEnumerable<UserLocation>> LoadUserLocation(int UserId);
        void DeleteUserLocation(int userID, int locationID);
        Task<bool> AssignUserLocation(int userID, int locationID);
    }
}
