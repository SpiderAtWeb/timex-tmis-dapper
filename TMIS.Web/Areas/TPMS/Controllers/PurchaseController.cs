using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TPMS.IRepository;
using TMIS.Models.ITIS;

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
    public IActionResult CreateRequest()
    {
      return View();
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> CreateRequest(DeviceType obj)
    {
      //if (await _deviceTypeRepository.CheckDeviceTypeExist(obj.DeviceTypeName!))
      //{
      //  ModelState.AddModelError("DeviceTypeName", "Device Type Already Available !");
      //}

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(obj);
      }

      // Insert  data if everything is valid
      //await _deviceTypeRepository.AddAsync(obj);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("DEVICE TYPE CREATED [" + obj.DeviceTypeName + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }
    #endregion
  }
}
