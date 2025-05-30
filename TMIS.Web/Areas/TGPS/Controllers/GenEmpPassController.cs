using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.SMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenEmpPassController(IEmployeePass db) : Controller
{
  private readonly ILog _logger = LogManager.GetLogger(typeof(ApprovalRequestController));
  private readonly IEmployeePass _db = db;

  public IActionResult Index()
  {    
    return View();
  }

  public async Task<IActionResult> Create()
  {
    var model = await _db.GetAllAsync();
    return View(model);
  }

  [HttpPost]
  public async Task<IActionResult> Create(EmployeePassVM employeePassVM)
  {
    await _db.InsertEmployeePassAsync(employeePassVM); ;
    return RedirectToAction("Index");
  }

}
