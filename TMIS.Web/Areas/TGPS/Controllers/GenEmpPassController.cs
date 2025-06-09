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
    if (employeePassVM == null)
    {
      _logger.Error("EmployeePassVM is null in Create method.");
      ModelState.AddModelError("", "Unexpected error occurred.");
      return await ReturnViewWithDropdowns();
    }   

    if (employeePassVM.EmployeePass.GuardRoomId <= 0)
    {
      _logger.Error("Invalid GuardRoomId.");
      ModelState.AddModelError("EmployeePass.GuardRoomId", "Please select a valid guard room.");
      return await ReturnViewWithDropdowns();
    }

    if (employeePassVM.EmployeePass.ApprovedById <= 0)
    {
      _logger.Error("Invalid ApprovedById.");
      ModelState.AddModelError("EmployeePass.ApprovedById", "Please select a valid approver.");
      return await ReturnViewWithDropdowns();
    }

    if (string.IsNullOrWhiteSpace(employeePassVM.EmployeePass.Location))
    {
      _logger.Error("Location is null or empty.");
      ModelState.AddModelError("EmployeePass.Location", "Location cannot be empty.");
      return await ReturnViewWithDropdowns();
    }

    if (string.IsNullOrWhiteSpace(employeePassVM.EmployeePass.Reason))
    {
      _logger.Error("Reason is null or empty.");
      ModelState.AddModelError("EmployeePass.Reason", "Reason cannot be empty.");
      return await ReturnViewWithDropdowns();
    }

    if (string.IsNullOrWhiteSpace(employeePassVM.EmployeePass.OutTime))
    {
      _logger.Error("OutTime is null or empty.");
      ModelState.AddModelError("EmployeePass.OutTime", "Out time cannot be empty.");
      return await ReturnViewWithDropdowns();
    }

    if (employeePassVM.EmployeePass.EmpPassEmpList == null || !employeePassVM.EmployeePass.EmpPassEmpList.Any())
    {
      _logger.Error("EmployeePass.EmpPassEmpList is null or empty.");
      ModelState.AddModelError("EmployeePass.EmpPassEmpList", "Employee list cannot be empty.");
      return await ReturnViewWithDropdowns();
    }

    for (int i = 0; i < employeePassVM.EmployeePass.EmpPassEmpList.Count; i++)
    {
      var emp = employeePassVM.EmployeePass.EmpPassEmpList[i];

      if (string.IsNullOrWhiteSpace(emp.EmpName))
      {
        _logger.Error("Empty employee name.");
        ModelState.AddModelError($"EmployeePass.EmpPassEmpList[{i}].EmpName", "Employee name is required.");
      }

      if (string.IsNullOrWhiteSpace(emp.EmpEPF) || !int.TryParse(emp.EmpEPF, out _))
      {
        _logger.Error($"Invalid EmpEPF: {emp.EmpEPF}");
        ModelState.AddModelError($"EmployeePass.EmpPassEmpList[{i}].EmpEPF", "Valid EPF is required.");
      }
    }

    if (!ModelState.IsValid)
      return await ReturnViewWithDropdowns();

    var result = await _db.InsertEmployeePassAsync(employeePassVM);
    TempData["success"] = result + " Exit pass created successfully.";
    return RedirectToAction("Index");
  }

  private async Task<IActionResult> ReturnViewWithDropdowns()
  {
    var viewModel = await _db.GetAllAsync(); // should return EmployeePassVM with GuardRooms and ApprovEmps populated
    return View("Create", viewModel);
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
