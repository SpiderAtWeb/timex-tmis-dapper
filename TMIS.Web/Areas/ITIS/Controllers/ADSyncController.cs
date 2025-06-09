using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class ADSyncController(ISessionHelper sessionHelper, ILdapServiceRepository ldapService) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly ILdapServiceRepository _ldapService = ldapService;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT ADSync INDEX");

      bool buttonStatus = await _ldapService.ButtonStatus("SYNCBTN");

      if (buttonStatus)
      {
        TempData["success"] = "SYNCING IS ALREADY RUN";
      }

      ViewBag.ButtonStatus = buttonStatus;
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> SyncADToDatabase()
    {
      bool isSuccess = await _ldapService.GetEmployeesFromAD();

      if (isSuccess)
      {
        TempData["success"] = "Record Synced Successfully";
        _logger.Info("AD DATA SYNCED - [" + _iSessionHelper.GetShortName() + "]");
      }
      else
      {
        TempData["error"] = "Failed to sync records from Active Directory.";
        _logger.Error("AD DATA SYNC FAILED - [" + _iSessionHelper.GetShortName() + "]");
      }

      return RedirectToAction("Index");
    }
  }
}
