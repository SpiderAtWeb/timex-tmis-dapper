using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TTMS.Controllers
{
  [Area("TTMS")]
  public class ToursController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult EmpVehicleAssign()
    {
      return View();
    }


  }
}
