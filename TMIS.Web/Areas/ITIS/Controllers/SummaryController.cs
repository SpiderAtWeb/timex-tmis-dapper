using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class SummaryController(ISessionHelper sessionHelper, IReportRepository reportRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(SummaryController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IReportRepository _reportRepository = reportRepository;
    public IActionResult Index()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");
      return View();
    }

    public IActionResult OveralReport()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT OVERAL REPORT");
      return View();
    }

    public IActionResult Analytics()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT ANALYTICS");
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - GET ALL DEVICES");
      var data = await _reportRepository.GetDeviceDetail();
      var jsonConverted = Json(data);
      return jsonConverted;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDataList(string callback)
    {
      var data = await _reportRepository.GetAllDeviceData();
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

    public IActionResult DeviceCount()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT DEVICE COUNT");
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDevicesCount(string callback)
    {
      var data = await _reportRepository.GetAllDevicesCount();
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
  }
}
