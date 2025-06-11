using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class MachineDisposalController(IDisposal db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(MachineDisposalController));
    private readonly IDisposal _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      IEnumerable<TransMC> trlist = await _db.GetList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Details(int id)
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT Details");

      var oMachine = await _db.GetMachineData(id);
      if (oMachine == null)
      {
        return NotFound();
      }

      return View(oMachine);
    }

    [HttpPost]
    public async Task<IActionResult> Details(MachinesData oMachinesData)
    {

      if (string.IsNullOrWhiteSpace(oMachinesData.Comments))
      {
        ModelState.AddModelError("Comments", "Disposals Remark is Required  !");
        return View(oMachinesData);
      }

      if (oMachinesData.Comments.Length >= 50)
      {
        ModelState.AddModelError("Comments", "Limit Your Comment to 50 Characters !");
        return View(oMachinesData);
      }


      await _db.SaveMachineObsoleteAsync(oMachinesData!);

      TempData["success"] = "Record created successfully";

      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - DISPOSED [" + oMachinesData!.QrCode + "]");
      return RedirectToAction("Index");
    }
  }
}
