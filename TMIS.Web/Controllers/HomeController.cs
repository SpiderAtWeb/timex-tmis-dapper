using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models;
using TMIS.Models.Auth;

namespace TMIS.Controllers
{
  [Authorize]
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class HomeController(IUserAccess userAccess, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));
    private readonly IUserAccess _userAccess = userAccess;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {
      var userRole = User.IsInRole("admin") ? "admin" : "User";
      ViewBag.UserRole = userRole;

      return View();
    }

    public IActionResult Profile()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Profile(PasswordChange passwordChange)
    {
      if (passwordChange == null)
      {
        return BadRequest("Invalid request.");
      }

      //add validation error
      if (string.IsNullOrEmpty(passwordChange.Password) ||
        string.IsNullOrEmpty(passwordChange.NewPassword) ||
        string.IsNullOrEmpty(passwordChange.ConfirmPassword))
      {
        ModelState.AddModelError("Password", "Password is required.");
        ModelState.AddModelError("NewPassword", "New Password is required.");
        ModelState.AddModelError("ConfirmPassword", "Confirm Password is required.");
      }
      else
      {
        if (passwordChange.NewPassword != passwordChange.ConfirmPassword)
        {
          ModelState.AddModelError("ConfirmPassword", "Password does not match.");
        }
      }

      if (!ModelState.IsValid)
      {
        return View(passwordChange);  // This will preserve the form values
      }

      var isUpdated = await _userAccess.UpdateUserPassword(passwordChange.Password!, passwordChange.ConfirmPassword!);

      if (isUpdated)
      {
        // Show success message and redirect
        TempData["success"] = "Password Updated successfully";

        _logger.Info("PASSWORD UPDATE SUCCEFULLY - [" + _iSessionHelper.GetShortName() + "]");

        return RedirectToAction("Index");
      }
      else
      {
        ModelState.AddModelError("ConfirmPassword", "Password update failed");
        return View();
      }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


  }
}
