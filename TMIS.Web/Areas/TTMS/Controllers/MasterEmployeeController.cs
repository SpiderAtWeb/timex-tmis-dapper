using Microsoft.AspNetCore.Mvc;

namespace TMIS.Areas.TTMS.Controllers
{
  public class MasterEmployeeController : Controller
  {
    public IActionResult Employees()
    {
      return View();
    }

    public IActionResult Vehicles()
    {
      return View();
    }

    public IActionResult PaymentTerms()
    {
      return View();
    }
  }
}
