using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class DeviceTypeController(IDeviceTypeRepository deviceTypeRepository, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceTypeController));
    private readonly IDeviceTypeRepository _deviceTypeRepository = deviceTypeRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT TYPE INDEX");
      var deviceTypes = await _deviceTypeRepository.GetAllAsync();
      return View(deviceTypes);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var deviceEditVM = await _deviceTypeRepository.LoadDeviceType(id);

      if (deviceEditVM == null)
      {
        return NotFound();
      }

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE TYPE EDIT [" + deviceEditVM.DeviceTypeID + "]");
      return View(deviceEditVM);
    }

    public async Task<IActionResult> Delete(int id)
    {
      var deviceEditVM = await _deviceTypeRepository.LoadDeviceType(id);

      if (deviceEditVM == null)
      {
        return NotFound();
      }

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE TYPE DELETE [" + deviceEditVM.DeviceTypeID + "]");
      return View(deviceEditVM);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(DeviceType obj)
    {
      // Update machine data if everything is valid
      bool isdelete = await _deviceTypeRepository.DeleteDeviceType(obj.DeviceTypeID);

      if (isdelete)
      {
        // Show success message and redirect
        TempData["success"] = "Record Deleted Successfully";
        _logger.Info("DEVICE TYPE DELETED [" + obj.DeviceTypeName + "] - [" + _iSessionHelper.GetShortName() + "]");
      }
           
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> View(int id)
    {
      var deviceEditVM = await _deviceTypeRepository.LoadDeviceType(id);

      if (deviceEditVM == null)
      {
        return NotFound();
      }

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT DEVICE TYPE VIEW [" + deviceEditVM.DeviceTypeID + "]");
      return View(deviceEditVM);
    }
    public IActionResult Create()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT TYPE CREATE");
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DeviceType obj, IFormFile? image)
    {
      var deviceEditVM = await _deviceTypeRepository.LoadDeviceType(obj.DeviceTypeID);

      if (await _deviceTypeRepository.CheckDeviceTypeExist(obj))
      {
        ModelState.AddModelError("DeviceTypeName", "Device Type Already Available !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {    
        return View(deviceEditVM);
      }

      // Update machine data if everything is valid
      await _deviceTypeRepository.UpdateDeviceType(obj, image);

      // Show success message and redirect
      TempData["success"] = "Record Updated Successfully";

      _logger.Info("DEVICE TYPE UPDATED [" + obj.DeviceTypeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Create(DeviceType obj, IFormFile? image)
    {
      if(await _deviceTypeRepository.CheckDeviceTypeExist(obj.DeviceTypeName!))
      {
        ModelState.AddModelError("DeviceTypeName", "Device Type Already Available !");
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(obj);
      }

      // Insert machine data if everything is valid
      await _deviceTypeRepository.AddAsync(obj, image);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("DEVICE TYPE CREATED [" + obj.DeviceTypeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");    
    }
  }
}
