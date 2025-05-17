using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenEmpPassController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
