using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.DataAccess.PLMS.Rpository;
using TMIS.Models.PLMS;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class NextStagesController(ICommon common, INextStages nextStages, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(NextStagesController));
    private readonly ICommon _common = common;
    private readonly INextStages _nextStages = nextStages;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _common.GetInquiriesAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> CreateNext(string id)
    {
      var oInquiriesVM = await _nextStages.LoadNextInquiryDropDowns(id);
      return View(oInquiriesVM);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNext(Models.PLMS.NewInquiryVM inquiryVM, IFormFile? artwork)
    {
      var result = await _nextStages.SaveNextInquiryAsync(inquiryVM, artwork);
      TempData["success"] = result;
      return RedirectToAction("Index");
    }
  }
}
