using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class OverviewController(IDashBoard db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(OverviewController));
    private readonly IDashBoard _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {     
      return View();
    }

    public async Task<IActionResult> Summary()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      var vMCluster = new VMCluster
      {
        OwnedClusterList = await _db.GetClusterDetails(),

      };
      return View(vMCluster);
    }

    public IActionResult Details()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT DETAILS");
      return View();
    }

    public async Task<IActionResult> Pivot()
    {
      var vMCluster = new VMCluster
      {
        OwnedClusterList = await _db.GetClusterDetails(),

      };

      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT PIVOT");

      return View(vMCluster);
    }

    #region API Calls
    public IActionResult History(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        return BadRequest("McId is required.");
      }

      string[] logData = _db.GetTrLoggerData(id);

      // Log the visit details
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT DETAILS ");

      // Return the log data as a JSON response
      return Json(new { logs = logData });
    }

    public async Task<IActionResult> GetSumryData(string clusterId)
    {
      var data = await _db.GetSmryDataAsync(clusterId);
      return Json(new { data });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDataList(string callback)
    {
      var data = await _db.GetAllInventoryData();
      var jsonResponse = JsonConvert.SerializeObject(data);

      if (!string.IsNullOrEmpty(callback))
      {
        return Content($"{callback}({jsonResponse});", "application/javascript");
      }
      else
      {
        return Json(data);
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetPivotData(string callback, string cluster)
    {
      var data = await _db.GetPivotData(cluster);
      var jsonResponse = JsonConvert.SerializeObject(data);

      if (!string.IsNullOrEmpty(callback))
      {
        return Content($"{callback}({jsonResponse});", "application/javascript");
      }
      else
      {
        return Json(data);
      }
    }
    [HttpGet("GetMachineStatus")]
    public async Task<IActionResult> CostGetAll()
    {
      var oDashboard = await _db.GetDashBoardData();

      // Return the data as JSON
      return Ok(oDashboard);
    }

    #endregion
  }
}
