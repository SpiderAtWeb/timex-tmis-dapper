using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TAPS.IRepository;
using TMIS.Models.TAPS.VM;

namespace TMIS.Areas.TAPS.Controllers
{
  [Area("TAPS")]
  public class NewUserController(ISessionHelper sessionHelper, IAdminRepository adminRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(NewUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IAdminRepository _adminRepository = adminRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT NEWUSER INDEX");  

      NewUserVM newUserVM = new();
      newUserVM.UserEmailList = await _adminRepository.LoadEmployeeList();
      newUserVM.LocationList = await _adminRepository.LoadLocationList();
      return View(newUserVM);
    }

    [HttpPost]
    public async Task<IActionResult> Index(NewUserVM obj)
    {
      if (await _adminRepository.CheckUserEmailExist(obj.UserEmail!))
      {
        ModelState.AddModelError("UserEmail", "User Already In The System !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        obj.UserEmailList = await _adminRepository.LoadEmployeeList();
        obj.LocationList = await _adminRepository.LoadLocationList();
        return View(obj);
      }

      // Insert User if everything is valid
      bool result = await _adminRepository.InsertNewUser(obj);

      if (result)
      {
        // Show success message and redirect
        TempData["success"] = "Record Created Successfully";

        _logger.Info("NEW USER CREATED [" + obj.UserEmail + "] - [" + _iSessionHelper.GetShortName() + "]");
      }

      return RedirectToAction("Index");
    }
  }
}
