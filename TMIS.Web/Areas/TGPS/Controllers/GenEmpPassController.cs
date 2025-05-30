using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.SMIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenEmpPassController(IEmployeePass db) : BaseController
{
  private readonly ILog _logger = LogManager.GetLogger(typeof(ApprovalRequestController));
  private readonly IEmployeePass _db = db;

  public IActionResult Index()
  {
    var empPassList = _db.GetList().Result;
    return View(empPassList);
  }

  public async Task<IActionResult> Create()
  {
    var model = await _db.GetAllAsync();
    return View(model);
  }

  [HttpPost]
  public async Task<IActionResult> Create(EmployeePassVM employeePassVM)
  {
    var result =  await _db.InsertEmployeePassAsync(employeePassVM);
    TempData["Success"] = result + " Exit pass created successfully.";
    return RedirectToAction("Index");
  }

  [HttpGet]
  public async Task<IActionResult> GetGatePassDetails(int id)
  {
    var result = await _db.GetEmpPassesAsync(id);
    if (result == null)
    {
      _logger.Error($"No gatepass found with ID: {id}");
      return NotFound("Gatepass not found.");
    }
    return PartialView("_GatePassDetailsPartial", result);
  }

}
