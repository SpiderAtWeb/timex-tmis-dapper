using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.DataAccess.HRRS.Repository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.Auth;
using TMIS.Models.HRRS.VM;

namespace TMIS.Areas.HRRS.Controllers
{
  [Area("HRRS")]
  public class ITRequestController(ISessionHelper sessionHelper, IITRequestRepository iTRequestRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(ITRequestController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IITRequestRepository _iTRequestRepository = iTRequestRepository;

    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT ITREQUEST INDEX");      
      var Obj = await _iTRequestRepository.GetAllAsync();
      return View(Obj);
    }
    public async Task<IActionResult> Edit(int id)
    {
      
      ITRequestPageViewModel obj = new();
      obj.CreateObj.HRRS_ITRequest = await _iTRequestRepository.LoadRequest(id);
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

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT IT REQUEST EDIT [" + id + "]");
      TempData["RequestId"] = id;
      return View(obj);
    }
    public async Task<IActionResult> Create()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT ITREQUEST CREATE");

      ITRequestPageViewModel obj = new();      
      obj.CreateObj = await _iTRequestRepository.LoadDropDowns();

      return View(obj);
    }
    [HttpPost]
    public async Task<IActionResult> Create(ITRequestPageViewModel obj)
    {
      Create objnew = await _iTRequestRepository.LoadDropDowns();            
      obj.CreateObj.LocationList = objnew.LocationList;
      obj.CreateObj.DepartmentList = objnew.DepartmentList;
      obj.CreateObj.DesignationList = objnew.DesignationList;
      obj.CreateObj.EmployeeList = objnew.EmployeeList;

      if (obj.CreateObj.HRRS_ITRequest!.IsReplacement == false)
      {
        obj.CreateObj.HRRS_ITRequest.Replacement = null; // Clear Replacement if not applicable
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.Replacement))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.Replacement", "Replacement is required.");
        }
      }
      if (obj.CreateObj.HRRS_ITRequest!.Email == false)
      {
        obj.CreateObj.HRRS_ITRequest.EmailGroup = null; // Clear Replacement if not applicable
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.EmailGroup))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.EmailGroup", "Email Group is required.");
        }
      }
      if (obj.CreateObj.HRRS_ITRequest.SIM == false)
      {
        obj.CreateObj.HRRS_ITRequest.HomeAddress = null; // Clear HomeAddress if SIM is not Yes
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.HomeAddress))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.HomeAddress", "Home Address is required.");
        }
      }

      if (!ModelState.IsValid)
      {       
        return View(obj);
      }

      obj.CreateObj.HRRS_ITRequest!.FirstName = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.FirstName);
      obj.CreateObj.HRRS_ITRequest!.LastName = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.LastName);
      obj.CreateObj.HRRS_ITRequest!.Designation = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Designation);
      obj.CreateObj.HRRS_ITRequest!.Department = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Department);
      obj.CreateObj.HRRS_ITRequest!.Location = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Location);
      obj.CreateObj.HRRS_ITRequest!.Company = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Company);

      bool isAdded = await _iTRequestRepository.AddAsync(obj.CreateObj);

      if (isAdded)
      {
        // Show success message and redirect
        TempData["success"] = "IT Request Successful";
      }
      else {         // Show error message
        TempData["error"] = "Something went wrong, please try again.";
      }

      return RedirectToAction("Create");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ITRequestPageViewModel obj)
    {
      Create objnew = await _iTRequestRepository.LoadDropDowns();
      obj.CreateObj.LocationList = objnew.LocationList;
      obj.CreateObj.DepartmentList = objnew.DepartmentList;
      obj.CreateObj.DesignationList = objnew.DesignationList;
      obj.CreateObj.EmployeeList = objnew.EmployeeList;
     
      var itRequest = await _iTRequestRepository.LoadRequest(obj.CreateObj.HRRS_ITRequest!.RequestID);

      if(itRequest == null)
      {
        TempData["error"] = "Something went wrong, please try again.";
        return RedirectToAction("Index");
      }

      if (obj.CreateObj.HRRS_ITRequest!.Email == false)
      {
        obj.CreateObj.HRRS_ITRequest.EmailGroup = null; // Clear Replacement if not applicable
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.EmailGroup))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.EmailGroup", "Email Group is required.");
        }
      }
      if (obj.CreateObj.HRRS_ITRequest!.IsReplacement == false)
      {
        obj.CreateObj.HRRS_ITRequest.Replacement = null; // Clear Replacement if not applicable
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.Replacement))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.Replacement", "Replacement is required.");
        }
      }
      if (obj.CreateObj.HRRS_ITRequest.SIM == false)
      {
        obj.CreateObj.HRRS_ITRequest.HomeAddress = null; // Clear HomeAddress if SIM is not Yes
      }
      else
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.HomeAddress))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.HomeAddress", "Home Address is required.");
        }
      }

      if (!ModelState.IsValid)
      {
        return View(obj);
      }

      obj.CreateObj.HRRS_ITRequest!.FirstName = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.FirstName);
      obj.CreateObj.HRRS_ITRequest!.LastName = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.LastName);
      obj.CreateObj.HRRS_ITRequest!.Designation = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Designation);
      obj.CreateObj.HRRS_ITRequest!.Department = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Department);
      obj.CreateObj.HRRS_ITRequest!.Location = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Location);
      obj.CreateObj.HRRS_ITRequest!.Company = CapitalizeFirstLetter(obj.CreateObj.HRRS_ITRequest.Company);
      
      bool isAdded = await _iTRequestRepository.UpdateAsync(obj.CreateObj.HRRS_ITRequest);

      if (isAdded)
      {
        // Show success message and redirect
        TempData["success"] = "IT Request Edit Successful";
      }
      else
      {
        // Show error message
        TempData["error"] = "Something went wrong, please try again.";
      }

      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete()
    {
      var id = Convert.ToInt32(TempData["RequestId"]);
      var itRequest = await _iTRequestRepository.LoadRequest(id);
      if (itRequest == null)
      {
        return NotFound();
      }
      bool isDeleted = await _iTRequestRepository.DeleteAsync(itRequest.RequestID);
      if (isDeleted)
      {
        TempData["success"] = "IT Request Deleted Successfully";
        _logger.Info("IT REQUEST DELETED [" + itRequest.FirstName + " " + itRequest.LastName + "] - [" + _iSessionHelper.GetShortName() + "]");
      }
      else
      {
        TempData["error"] = "Something went wrong, please try again.";
      }
      return RedirectToAction("Index");
    }
    private string? CapitalizeFirstLetter(string? input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return input;

      input = input.Trim().ToLower(); // Make all lowercase first
      return char.ToUpper(input[0]) + input.Substring(1);
    }
    public IActionResult SendEmail(int id)
    {
      try
      {
        _iTRequestRepository.PrepairEmail(id);       
        TempData["success"] = "Request Email Send Successful";
      }
      catch (Exception ex)
      {
        TempData["error"] = "Something went wrong, please try again.: " + ex.Message;        
      }

      return RedirectToAction("Index");

    }
  }
}
