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
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICEUSER INDEX");
      var deviceUserVM = await _deviceUserRepository.GetAllAsync(); 
      return View(deviceUserVM);
    }

    public async Task<IActionResult> Create()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICEUSER CREATE");
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
      // load dropdown list before validations
      var deviceUserVM = await _deviceUserRepository.LoadDropDowns();

      if (obj.AssignDevice!.Device == 0)
      {
        ModelState.AddModelError("AssignDevice.Device", "The Device field is required.");        
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(deviceUserVM);
      }

      // Insert machine data if everything is valid
      await _deviceUserRepository.AddAsync(obj);

      // Show success message and redirect
      TempData["success"] = "Assign Successfully";

      _logger.Info("DEVICE ASSIGN [" + obj.AssignDevice.Device + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Return()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICEUSER RETURN");
      var deviceUserVM = await _deviceUserRepository.LoadInUseDevices();
      return View(deviceUserVM);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserDetails(int deviceId)
    {
      var deviceDetails = await _deviceUserRepository.LoadUserDetail(deviceId);
      return Json(deviceDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Return(ReturnDeviceVM obj, IFormFile? image)
    {
      var deviceUserVM = await _deviceUserRepository.LoadInUseDevices();

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        deviceUserVM.Device = null;
        return View(deviceUserVM);
      }

      bool returnedDevice = await _deviceUserRepository.ReturnDevice(obj, image);
      if (returnedDevice)
      {
        TempData["success"] = "Returned Successfully";

        _logger.Info("DEVICE RETURNED[" + obj.Device + "] - [" + _iSessionHelper.GetShortName() + "]");

        return RedirectToAction("Index");
      }

      deviceUserVM.Device = null;
      return View(deviceUserVM);
    }

  }
}
