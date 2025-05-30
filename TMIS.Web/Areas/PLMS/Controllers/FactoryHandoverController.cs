using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class FactoryHandoverController : BaseController
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
