using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.Auth;

namespace TMIS.Areas.Auth.Controllers
{
  [Area("Auth")]
  public class AccountController(IUserAccess userAccess, ISessionHelper sessionHelper) : Controller
  {
    private readonly IUserAccess _userAccess = userAccess;
    private readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(InputModel inputModel)
    {
      _logger.Info("LOGGIN ATTEMPT - UN [" + inputModel.Email + "] – PW [" + inputModel.Password + "]");

      if (ModelState.IsValid)
      {
        // Fetch the user from the database using Dapper
        var user = await _userAccess.GetUserByUsernameAsync(inputModel.Email, inputModel.Password);

        if (user.UserRole != "")
        {
          if (user.AccessPlants!.Length <= 0)
          {
            ModelState.AddModelError(string.Empty, "No units have been assigned to the user. !! Contact System Admin");
            _logger.Error("NO UNITS ASSIGN -  [" + inputModel.Email + "] – user password [" + inputModel.Password + "]");
            return View();
          }

          // Create claims and claims identity
          var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.NameWi),
                new(ClaimTypes.Role,user.UserRole),
                new(ClaimTypes.Email, inputModel.Email)
            };

          var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

          // Create a claims principal
          var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

          // Sign in the user
          await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
          {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
          });
          _logger.Info("USER SIGN IN UN -  [" + inputModel.Email + "] – PW [" + inputModel.Password + "]");


          // Redirect to the home page after successful login
          return RedirectToAction("Index", "Home", new { Area = "" });
        }

        // If authentication fails, return to the login page with an error message
        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        _logger.Error("USER INVALID MSG -  [" + inputModel.Email + "] – user password [" + inputModel.Password + "]");

      }
      return View();
    }

    // Logout action
    public async Task<IActionResult> Logout()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - USER SIGN OUT");

      // Sign out the user
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      _iSessionHelper.ClearSession();    

      return RedirectToAction("Index", "Home", new { Area = "" });
    }

    // Access denied page for unauthorized access
    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}
