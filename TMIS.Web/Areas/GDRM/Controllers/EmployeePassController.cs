using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.Models.GDRM;

namespace TMIS.Areas.GDRM.Controllers
{
  [Area("GDRM")]
  public class EmployeePassController(IGREmployee db) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(EmployeePassController));
    private readonly IGREmployee _db = db;

    public async Task<IActionResult> Index()
    {
      var empPendings = await _db.GetEmployeePendingList();
      return View(empPendings);
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeeGatepassDetails(int id)
    {
      var empGatepass = await _db.GetEmployeeGatepassByIdAsync(id);
      return Json(empGatepass);
    }

    [HttpPost]
    public async Task<IActionResult> EmpGPUpdate([FromBody] EmpGpUpdate empGpUpdate)
    {
      var result = await _db.EmployeeGatePassUpdating(empGpUpdate);
      return Json(result);
    }
  }
}
