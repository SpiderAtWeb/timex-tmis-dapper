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
  }
}
