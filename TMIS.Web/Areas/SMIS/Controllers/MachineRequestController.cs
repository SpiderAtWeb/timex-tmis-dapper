using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Helper;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Authorize]
  [Area("SMIS")]
  public class MachineRequestController(ITransfers db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ITransfers _db = db;
    private readonly ILog _logger = LogManager.GetLogger(typeof(MachineRequestController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      ViewBag.today = DateTime.Now;
      IEnumerable<TransMC> trlist = await _db.GetList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Details(int Id)
    {
      var oDetails = new McRequestDetailsVM
      {
        oMcData = await _db.GetMachineData(Id),
        LocationList = await _db.GetLocationsList(),
        UnitsList = await _db.GetUnitsList()
      };

      if (oDetails.oMcData == null)
      {
        return NotFound();
      }
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT DETAILS");
      return View(oDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Details(McRequestDetailsVM mcRequestDetailsVM)
    {
      MachineValidator.ValidateTrasnferMachine(mcRequestDetailsVM, ModelState);
      if (!ModelState.IsValid)
      {
        mcRequestDetailsVM.LocationList = await _db.GetLocationsList();
        mcRequestDetailsVM.UnitsList = await _db.GetUnitsList();
        return View("Details", mcRequestDetailsVM);
      }

      await _db.SaveMachineTransferAsync(mcRequestDetailsVM!);

      TempData["success"] = "Record Created Successfully";

      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - REQUEST CREATED [" + mcRequestDetailsVM!.oMcData!.QrCode + "]");
      return RedirectToAction("Index");
    }

    #region API Calls
    [HttpGet]
    public async Task<IActionResult> MyRequestGetAll(string dateTr)
    {
      IEnumerable<TransMCUser> Trlist = await _db.GetListUser(dateTr);
      return Json(new { data = Trlist });
    }
    #endregion
  }
}
