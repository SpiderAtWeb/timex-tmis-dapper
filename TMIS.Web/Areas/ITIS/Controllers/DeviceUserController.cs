using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.ITIS.Controllers
{
  public class DeviceUserController : Controller
  {
    public async Task<IActionResult> Index()
    {
      return View();
    }
  }
}
