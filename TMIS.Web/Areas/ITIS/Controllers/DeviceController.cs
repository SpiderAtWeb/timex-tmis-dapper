using log4net;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class DeviceController(IDeviceRepository deviceRepository, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceTypeController));
    private readonly IDeviceRepository _deviceRepository = deviceRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE INDEX");
      var devices = await _deviceRepository.GetAllAsync();
      return View(devices);
    }
    public async Task<IActionResult> Create()
    {
      var createDeviceVM = await _deviceRepository.LoadDropDowns(null);
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE CREATE");

      return View(createDeviceVM);
    }

    public async Task<IActionResult> View(int deviceID)
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE VIEW");

      var deviceDetails = await _deviceRepository.LoadDeviceDetail(deviceID);
      var userDetails = await _deviceRepository.LoadUserDetail(deviceID);

      ViewDeviceVM viewDeviceVM = new ViewDeviceVM();

      viewDeviceVM.DeviceUserDetail = userDetails;
      viewDeviceVM.DeviceDetail = deviceDetails;

      return View(viewDeviceVM);
    }

    public async Task<IActionResult> Edit(int deviceID)
    {
      var createDeviceVM = await _deviceRepository.LoadDropDowns(deviceID);
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE Edit");

      return View(createDeviceVM);
    }

    #region APICaLL

    [HttpPost]
    public async Task<IActionResult> Edit(CreateDeviceVM obj, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4)
    {
      var createDeviceVM = await _deviceRepository.LoadDropDowns(obj.Device!.DeviceID);

      if (obj.Device!.SerialNumber != null && obj.Device!.SerialNumber.Any())
      {
        if (await _deviceRepository.CheckSerialEdit(obj.Device!.SerialNumber, obj.Device!.DeviceID))
        {
          ModelState.AddModelError("Device.SerialNumber", "Serial Number Already Available !");
        }
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(createDeviceVM);
      }


      // Insert Device
      bool record = await _deviceRepository.UpdateDevice(obj, image1, image2, image3, image4);

      if (!record)
      {
        return View(createDeviceVM);
      }

      // Show success message and redirect
      TempData["success"] = "Record Update Successfully";

      _logger.Info("DEVICE UPDATED [" + obj.Device.SerialNumber + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDeviceVM obj, IFormFile? image1, IFormFile? image2, IFormFile? image3, IFormFile? image4)
    {
      // Load the necessary lists before validation
      var createDeviceVM = await _deviceRepository.LoadDropDowns(null);
      obj.LocationList = createDeviceVM.LocationList;
      obj.DeviceTypeList = createDeviceVM.DeviceTypeList;
      obj.DeviceStatusList = createDeviceVM.DeviceStatusList;
      obj.VendorsList = createDeviceVM.VendorsList;
      obj.DepartmentList = createDeviceVM.DepartmentList;      

      if (obj.Device!.SerialNumber != null)
      {
        if (await _deviceRepository.CheckSerialNumberExist(obj.Device.SerialNumber))
        {
          ModelState.AddModelError("Device.SerialNumber", "Serial Number Already Available !");
        }
      }
      if (obj.Device.DeviceTypeID == 0)
      {
        ModelState.AddModelError("Device.DeviceTypeID", "Device Type  field is required.");
      }
      if (image1 == null)
      {
        ModelState.AddModelError("Device.Image1Data", "Image 1 is required.");
      }
      if (obj.Device.DeviceStatusID == 0)
      {
        ModelState.AddModelError("Device.DeviceStatusID", "Device Status  field is required.");
      }
      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {       
        return View(obj);
      }

      // Insert Device
      bool record = await _deviceRepository.AddAsync(obj, image1, image2, image3, image4);

      if (!record)
      {
        return View(obj);
      }

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("DEVICE CREATED [" + obj.Device.SerialNumber + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetAttributesByDeviceType(int deviceTypeId)
    {
      var attributes = await _deviceRepository.GetAllAttributes(deviceTypeId);
      return Json(attributes);
    }

    #endregion
  }
}
