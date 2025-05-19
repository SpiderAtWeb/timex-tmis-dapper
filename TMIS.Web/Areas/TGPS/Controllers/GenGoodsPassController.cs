using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenGoodsPassController : Controller
{
  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Create()
  {
    return View();
  }
}
