using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;

namespace TMIS.Areas.TGPS.Controllers
{

  [Area("TGPS")]
  public class ResponseController(IResponse db) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(GenGoodsPassController));
    private readonly IResponse _db = db;
    public async Task<IActionResult> GoodsPass()
    {
      var result = await _db.GetList();
      return View(result);
    }

    public IActionResult EmpPass()
    {
      var empPassList = _db.GetEmpList().Result;
    return View(empPassList);
    }

    [HttpGet]
    public async Task<IActionResult> GetGGatePassDetails(int id)
    {
      var result = await _db.LoadShowGPDataAsync(id);
      if (result == null)
      {
        _logger.Error($"No gatepass found with ID: {id}");
        return NotFound("Gatepass not found.");
      }
      return PartialView("_GGatePassDetailsPartial", result);
    }

    [HttpPost]
    public async Task<IActionResult> HandleGGatePassAction(int id, string action)
    {
      var result = await _db.HandleGGpAction(id, action);
      if (!result)
      {
        _logger.Error($"Failed to handle gatepass action for ID: {id} with action: {action}");
        return BadRequest("Failed to handle gatepass action.");
      }

      return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetEGatePassDetails(int id)
    {
      var result = await _db.GetEmpPassesAsync(id);
      if (result == null)
      {
        _logger.Error($"No gatepass found with ID: {id}");
        return NotFound("Gatepass not found.");
      }
      return PartialView("_EGatePassDetailsPartial", result);
    }

    [HttpPost]
    public async Task<IActionResult> HandleEGatePassAction(int id, string action)
    {
      var result = await _db.HandleEGpAction(id, action);
      if (!result)
      {
        _logger.Error($"Failed to handle gatepass action for ID: {id} with action: {action}");
        return BadRequest("Failed to handle gatepass action.");
      }

      return Ok();
    }

  }
}
