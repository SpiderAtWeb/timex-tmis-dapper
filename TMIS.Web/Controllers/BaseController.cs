using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TMIS.Controllers
{
  public class BaseController : Controller
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      // Assume you store user info in session on login
      var userId = context.HttpContext.Session.GetString("UserId");

      if (string.IsNullOrEmpty(userId))
      {
        context.Result = new RedirectToActionResult("Login", "Account", new { area = "Auth" });
        return;
      }

      base.OnActionExecuting(context);
    }
  }
}
