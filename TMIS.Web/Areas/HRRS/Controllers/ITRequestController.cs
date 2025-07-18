using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using TMIS.Areas.ITIS.Controllers;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.DataAccess.HRRS.Repository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.HRRS.VM;

namespace TMIS.Areas.HRRS.Controllers
{
  [Area("HRRS")]
  public class ITRequestController(ISessionHelper sessionHelper, IITRequestRepository iTRequestRepository) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(ITRequestController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IITRequestRepository _iTRequestRepository = iTRequestRepository;

    public async Task<IActionResult> Create()
    {
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT ITREQUEST CREATE");

      ITRequestPageViewModel obj = new();

      obj.ITRequestTableObj = await _iTRequestRepository.GetAllAsync();
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

      if (obj.CreateObj.HRRS_ITRequest!.RecruitmentType != "Replacement")
      {
        obj.CreateObj.HRRS_ITRequest.Replacement = null; // Clear Replacement if not applicable
      }
      else if (obj.CreateObj.HRRS_ITRequest.RecruitmentType == "Replacement")
      {
        if (string.IsNullOrWhiteSpace(obj.CreateObj.HRRS_ITRequest.Replacement))
        {
          ModelState.AddModelError("CreateObj.HRRS_ITRequest.Replacement", "Replacement is required.");
        }
      }
      if (obj.CreateObj.HRRS_ITRequest.SIM != "Yes")
      {
        obj.CreateObj.HRRS_ITRequest.HomeAddress = null; // Clear HomeAddress if SIM is not Yes
      }
      else if (obj.CreateObj.HRRS_ITRequest.SIM == "Yes")
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

    private string? CapitalizeFirstLetter(string? input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return input;

      input = input.Trim().ToLower(); // Make all lowercase first
      return char.ToUpper(input[0]) + input.Substring(1);
    }
  }
}
