using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class TerminationRentController(ITerminationRent db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(TerminationRentController));
    private readonly ITerminationRent _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      IEnumerable<TransMC> trlist = await _db.GetList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Details(int id)
    {
      var oMachine = await _db.GetRentedMcById(id);
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT Details");
      if (oMachine == null)
      {
        return NotFound();
      }
      return View(oMachine);
    }

    [HttpPost]
    public async Task<IActionResult> Details(MachineRentedVM oMachinesData)
    {

      if (string.IsNullOrWhiteSpace(oMachinesData.RentTermRemark))
      {
        ModelState.AddModelError(nameof(MachineRentedVM.RentTermRemark), "Rent Termination Remark is Required  !");
        return View(oMachinesData);
      }

      if (oMachinesData.RentTermRemark.Length >= 50)
      {
        ModelState.AddModelError(nameof(MachineRentedVM.RentTermRemark), "Limit Your Comment to 50 Characters !");
        return View(oMachinesData);
      }

      await _db.SaveMachineObsoleteAsync(oMachinesData!);

      TempData["success"] = "Record created Updated";

      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - REQUEST CREATED [" + oMachinesData!.QrCode + "]");
      return RedirectToAction("Index");
    }


  }
}
