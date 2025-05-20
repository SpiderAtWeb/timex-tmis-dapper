using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class DeviceUserController(ISessionHelper sessionHelper, IDeviceUserRepository deviceUserRepository) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IDeviceUserRepository _deviceUserRepository = deviceUserRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT DEVICEUSER INDEX");
      var deviceUserVM = await _deviceUserRepository.GetAllAsync(); 
      return View(deviceUserVM);
    }

    public async Task<IActionResult> Create()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT DEVICEUSER CREATE");
      var deviceUserVM = await _deviceUserRepository.LoadDropDowns();    
      return View(deviceUserVM);
    }

    [HttpGet]
    public async Task<IActionResult> GetDeviceDetails(int deviceId)
    {
      var deviceDetails = await _deviceUserRepository.LoadDeviceDetail(deviceId);
      return Json(deviceDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDeviceUserVM obj)
    {
      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(obj);
      }

      // Insert machine data if everything is valid
      await _deviceUserRepository.AddAsync(obj);

      // Show success message and redirect
      TempData["success"] = "Assign Successfully";

      _logger.Info("DEVICE ASSIGN [" + obj.AssignDevice.Device + "] - [" + _iSessionHelper.GetUserName() + "]");

      return RedirectToAction("Index");
    }

  }
}
