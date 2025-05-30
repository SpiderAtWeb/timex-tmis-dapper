using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TGPS.Controllers
{

  [Area("TGPS")]
  public class ResponseController : Controller
  {
    public IActionResult GoodsPass()
    {
      return View();
    }

    public IActionResult EmpPass()
    {
      return View();
    }
  }
}
