using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenVisitorPassController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
