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
      var obj = await _iTRequestRepository.LoadDropDowns();
      return View(obj);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Create obj)
    {
      obj = await _iTRequestRepository.LoadDropDowns();

      if (!ModelState.IsValid)
      {       
        return View(obj);
      }

      return RedirectToAction("Create");
    }
  }
}
