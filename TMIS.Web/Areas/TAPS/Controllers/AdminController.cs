using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TAPS.IRepository;
using TMIS.Models.TAPS.VM;

namespace TMIS.Areas.TAPS.Controllers
{
  [Area("TAPS")]
  public class AdminController(ISessionHelper sessionHelper, IAdminRepository adminRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IAdminRepository _adminRepository = adminRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT TYPE INDEX");
      var roles = await _adminRepository.LoadUserRoles();
      var users = await _adminRepository.LoadUsers();

      UserRoleVM userRoleVM = new()
      {
        UserList = users,
        UserRoleList = roles
      };
      return View(userRoleVM);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserDetails(int userID)
    {
      var userRoles = await _adminRepository.LoadUserRole(userID);
      return Json(userRoles);
    }

    [HttpPost]
    public IActionResult UnAssignUserRole(int userId, int userRoleId)
    {    
      _adminRepository.DeleteUserRole(userId, userRoleId);
      return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Index(UserRoleVM userRole)
    {
      var roles = await _adminRepository.LoadUserRoles();
      var users = await _adminRepository.LoadUsers();

      UserRoleVM userRoleVM = new()
      {
        UserList = users,
        UserRoleList = roles
      };
      var exist = await _adminRepository.CheckRoleExistToUser(userRole.selectedUserID, userRole.selectedUserRoleID);

      if (exist)
      {
        ModelState.AddModelError("selectedUserRoleID", "This role is already assigned to the user.");
      }
      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(userRoleVM);
      }
      
      var rowAffected = await _adminRepository.AssignUserRole(userRole.selectedUserID, userRole.selectedUserRoleID);

      if (rowAffected)
      {
        TempData["success"] = "User Role Assigned Successfully";
        _logger.Info("USER ROLE ASSIGNED [" + userRole.selectedUserID + "] - [" + _iSessionHelper.GetShortName() + "]");
      }
      else
      {
        TempData["error"] = "Failed to Assign User Role";
        _logger.Error("FAILED TO ASSIGN USER ROLE [" + userRole.selectedUserID + "] - [" + _iSessionHelper.GetShortName() + "]");
      }
      return RedirectToAction("Index");
    }
  }
}
