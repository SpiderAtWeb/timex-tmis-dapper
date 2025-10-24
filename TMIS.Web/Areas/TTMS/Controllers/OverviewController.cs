using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TTMS.Controllers
{
  [Area("TTMS")]
  public class OverviewController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
