using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TPMS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.TPMS.VM;

namespace TMIS.Areas.TPMS.Controllers
{
  [Area("TPMS")]
  public class PurchaseController(IRequestRepository requestRepository, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceTypeController));
    private readonly IRequestRepository _requestRepository = requestRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Index()
    {
      var requests = await _requestRepository.GetAllAsync();
      return View(requests);
    }
    public async Task<IActionResult> CreateRequest()
    {
      var obj = await _requestRepository.LoadListItems();
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT CreateRequest");
      return View(obj);
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> CreateRequestExists(CreateRequestVM obj)
    {
      // Load the necessary lists before validation
      var lists = await _requestRepository.LoadListItems();
      obj.SerialNumberList = lists?.SerialNumberList;
      obj.UserList = lists?.UserList;      
      obj.DepartmentList = lists?.DepartmentList;
      obj.DesignationList = lists?.DesignationList;
      obj.UnitList = lists?.UnitList;

      //if (await .CheckDeviceTypeExist(obj.DeviceTypeName!))
      //{
      //  ModelState.AddModelError("DeviceTypeName", "Device Type Already Available !");
      //}

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        // explicitly return to CreateRequest.cshtml with model + validation errors
        return View("CreateRequest", obj);
      }

      // Insert  data if everything is valid
      await _requestRepository.AddAsync(obj.TPMS_PurchaseRequests!);

      // Show success message and redirect
      TempData["success"] = "Request Created Successfully";

      _logger.Info("REQUEST CREATED [" + obj.TPMS_PurchaseRequests!.RequestID + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequestNew(CreateRequestVM obj)
    {
      // Load the necessary lists before validation
      var lists = await _requestRepository.LoadListItems();
      obj.SerialNumberList = lists?.SerialNumberList;
      obj.UserList = lists?.UserList;      
      obj.DepartmentList = lists?.DepartmentList;
      obj.DesignationList = lists?.DesignationList;
      obj.UnitList = lists?.UnitList;

      //if (await .CheckDeviceTypeExist(obj.DeviceTypeName!))
      //{
      //  ModelState.AddModelError("DeviceTypeName", "Device Type Already Available !");
      //}

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        // explicitly return to CreateRequest.cshtml with model + validation errors
        return View("CreateRequest", obj);
      }

      // Insert  data if everything is valid
      await _requestRepository.AddAsync(obj.TPMS_PurchaseRequests!);

      // Show success message and redirect
      TempData["success"] = "Request Created Successfully";

      _logger.Info("REQUEST CREATED [" + obj.TPMS_PurchaseRequests!.RequestID + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }
    #endregion
  }
}
