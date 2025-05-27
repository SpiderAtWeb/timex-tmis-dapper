using System;
using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.ITIS.VM;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class ApproveDeviceUserController(ISessionHelper sessionHelper, IApproveRepository approveRepository) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceUserController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IApproveRepository _approveRepository = approveRepository;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT APPROVEDEVICEUSER INDEX");
      var deviceUserVM = await _approveRepository.GetAllAsync();
      return View(deviceUserVM);
    }

    public async Task<IActionResult> Approve(int assignmentID)
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT APPROVEDEVICEUSER APPROVE");
      var selectedRecord = await _approveRepository.GetSelectedRecord(assignmentID);
      return View(selectedRecord);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(ApproveVM obj, string action)
    {
      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(obj);
      }

      if (action == "Approve")
      {
        // Insert machine data if everything is valid
        await _approveRepository.AddAsync(obj);

        // Show success message and redirect
        TempData["success"] = "Record Approved Successfully";

        _logger.Info("DEVICE APPROVED [" + obj.DeviceID + "] - [" + _iSessionHelper.GetUserName() + "]");
       
      }
      else if (action == "Reject")
      {
        // Insert machine data if everything is valid
        await _approveRepository.Reject(obj);

        // Show success message and redirect
        TempData["success"] = "Record Rejected Successfully";

        _logger.Info("DEVICE REJECTED [" + obj.DeviceID + "] - [" + _iSessionHelper.GetUserName() + "]");

        
      }

      return RedirectToAction("Index");

    }

  }
}
