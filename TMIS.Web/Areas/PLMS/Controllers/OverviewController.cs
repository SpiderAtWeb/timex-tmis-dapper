using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class OverviewController(IOverview overview, ISessionHelper sessionHelper) : BaseController
  {

    private readonly ILog _logger = LogManager.GetLogger(typeof(OverviewController));
    private readonly IOverview _overview = overview;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDataList(string callback)
    {
      var data = await _overview.GetAllRunningInqsData();
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
