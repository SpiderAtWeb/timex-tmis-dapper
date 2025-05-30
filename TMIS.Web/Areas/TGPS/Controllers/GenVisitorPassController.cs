using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenVisitorPassController : BaseController
{
  public IActionResult Index()
  {
    return View();
  }
}
