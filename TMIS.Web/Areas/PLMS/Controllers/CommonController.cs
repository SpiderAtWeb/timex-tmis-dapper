using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.PLMS.IRpository;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]

  public class CommonController(ICommon common) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(CommonController));
    private readonly ICommon _common = common;

    public async Task<IActionResult> LoadModal(string Id)
    {
      var dynamicModel = await _common.LoadModalDataAsync(Id);
      return Json(dynamicModel);

    }
  }
}
