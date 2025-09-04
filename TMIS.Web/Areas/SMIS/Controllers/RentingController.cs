using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class RentingController(IRenting db, ISessionHelper sessionHelper) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(RentingController));
    private readonly IRenting _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Approval()
    {
      IEnumerable<TransMC> trlist = await _db.GetList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Payments()
    {
      IEnumerable<TransMC> trlist = await _db.GetListPayments();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Details(int id)
    {
      var oMachine = await _db.GetRentedMcById(id);

      if (oMachine == null)
      {
        return NotFound();
      }
      return View(oMachine);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(string remarks, string id, string action)
    {
      if (id == null)
      {
        return View();
      }

      bool isApproved = action == "approve";

      string[] updateRecord = await _db.UpdateStatus(remarks, id, isApproved);

      if (updateRecord[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - REQUEST [" + action + "]");

        TempData["success"] = updateRecord[1];
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - ERROR [" + action + "]");
        TempData["Error"] = updateRecord[1];
      }

      // Redirect to the Index action
      return RedirectToAction("Approval");
    }

    [HttpPost]
    public IActionResult GenerateCertificate([FromBody] SelectedRequest request)
    {
      if (request == null || request.SelectedIds == null || !request.SelectedIds.Any())
      {
        return BadRequest("Please select at least one machine.");
      }

      // Do your certificate generation logic...
      string ids = string.Join(",", request.SelectedIds);
      return RedirectToAction("Certificate", new { ids });
    }

    public async Task<IActionResult> Certificate(string ids)
    {
      //Need load data from request
      var selectedIds = ids.Split(',').Select(int.Parse).ToList();
      var  certificateData= await _db.GetMachinesByIdsAsync(selectedIds);

      return View(certificateData);
    }

  }
}
