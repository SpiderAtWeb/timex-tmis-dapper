using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class SummaryController(ISessionHelper sessionHelper, IReportRepository reportRepository) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(SummaryController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IReportRepository _reportRepository = reportRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[ " + _iSessionHelper.GetUserName() + " ] - PAGE VISIT INDEX");
      return View();
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
  }
}
