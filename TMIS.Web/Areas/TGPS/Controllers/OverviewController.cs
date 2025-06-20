using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;


namespace TMIS.Areas.TGPS.Controllers
{
  [Area("TGPS")]

  public class OverviewController(IGpOverview overview) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(OverviewController));
    private readonly IGpOverview _overview = overview;

    public IActionResult Index()
    {
      return View();
    }  

   [HttpGet]
    public async Task<IActionResult> GetAllDataList(string callback)
    {
      var data = await _overview.GetList();
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


