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
      var obj = await _iTRequestRepository.LoadDropDowns();
      return View(obj);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Create obj)
    {
      Create objnew = await _iTRequestRepository.LoadDropDowns();            
      obj.LocationList = objnew.LocationList;
      obj.DepartmentList = objnew.DepartmentList;
      obj.DesignationList = objnew.DesignationList;
      obj.EmployeeList = objnew.EmployeeList;

      if (!ModelState.IsValid)
      {       
        return View(obj);
      }

      obj.HRRS_ITRequest!.FirstName = CapitalizeFirstLetter(obj.HRRS_ITRequest.FirstName);
      obj.HRRS_ITRequest!.LastName = CapitalizeFirstLetter(obj.HRRS_ITRequest.LastName);
      obj.HRRS_ITRequest!.Designation = CapitalizeFirstLetter(obj.HRRS_ITRequest.Designation);
      obj.HRRS_ITRequest!.Department = CapitalizeFirstLetter(obj.HRRS_ITRequest.Department);
      obj.HRRS_ITRequest!.Location = CapitalizeFirstLetter(obj.HRRS_ITRequest.Location);
      obj.HRRS_ITRequest!.Company = CapitalizeFirstLetter(obj.HRRS_ITRequest.Company);

      if(obj.HRRS_ITRequest.RecruitmentType != "Replacement")
      {
        obj.HRRS_ITRequest.Replacement = null; // Clear Replacement if not applicable
      }
      if (obj.HRRS_ITRequest.SIM != "Yes")
      {
        obj.HRRS_ITRequest.HomeAddress = null; // Clear HomeAddress if SIM is not Yes
      }

      bool isAdded = await _iTRequestRepository.AddAsync(obj);

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
