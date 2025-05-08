using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class FactoryHandoverController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
