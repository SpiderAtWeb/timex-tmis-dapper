using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TAPS.IRepository;

namespace TMIS.Areas.TAPS.Controllers
{
  [Area("TAPS")]
  public class AdminController(ISessionHelper sessionHelper, IAdminRepository adminRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IAdminRepository _adminRepository = adminRepository;
    public IActionResult Index()
    {
      return View();
    }
  }
}
