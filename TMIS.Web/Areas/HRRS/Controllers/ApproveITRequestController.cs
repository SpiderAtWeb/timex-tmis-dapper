using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.DataAccess.HRRS.Repository;
using TMIS.Models.HRRS.VM;
using TMIS.Models.ITIS.VM;

namespace TMIS.Areas.HRRS.Controllers
{
  [Area("HRRS")]
  public class ApproveITRequestController(ISessionHelper sessionHelper, IITRequestRepository iTRequestRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(ITRequestController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IITRequestRepository _iTRequestRepository = iTRequestRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT ITREQUEST INDEX");
      var Obj = await _iTRequestRepository.GetAllAsync();

      var viewModel = Obj.Where(x => x.PropName == "Mail Sent").ToList();

      return View(viewModel);
    }

    public async Task<IActionResult> Approve(int requestID)
    {      
      ITRequestPageViewModel obj = new();
      obj.CreateObj.HRRS_ITRequest = await _iTRequestRepository.LoadRequest(requestID);
      Create objnew = await _iTRequestRepository.LoadDropDowns();
      obj.CreateObj.LocationList = objnew.LocationList;
      obj.CreateObj.DepartmentList = objnew.DepartmentList;
      obj.CreateObj.DesignationList = objnew.DesignationList;
      obj.CreateObj.EmployeeList = objnew.EmployeeList;

      if (obj.CreateObj.HRRS_ITRequest == null)
      {
        TempData["error"] = "This request already approved.";
        return RedirectToAction("Index");
      }

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT APPROVEITREQUEST APPROVE [" + requestID + "]");      
      return View(obj);
    }
    [HttpPost]
    public async Task<IActionResult> Approve(ITRequestPageViewModel obj, string action)
    {
     
      bool updated = false;
      int status = 0;

      if (action == "Approve")
      {
        status = 2;     
      }
      else if (action == "Reject")
      {
        status = 3;     
      }

      updated = await _iTRequestRepository.ApproveAsync(obj.CreateObj.HRRS_ITRequest!, status);

      if (updated)
      {
        TempData["success"] = "Record " + action + " Successfully";
        _logger.Info("IT REQUEST [" + action +"] [" + obj.CreateObj.HRRS_ITRequest!.RequestID + "] - [" + _iSessionHelper.GetShortName() + "]");
      }
      return RedirectToAction("Index");
    }
  }
}
