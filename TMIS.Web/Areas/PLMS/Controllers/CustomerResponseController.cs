using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class CustomerResponseController(ICommon common, IFeedback feedback, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(CustomerResponseController));
    private readonly ICommon _common = common;
    private readonly IFeedback _feedback = feedback;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _common.GetInquiriesAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> Feedback(string id)
    {
      var inquiries = await _feedback.GetInquiryAsync(id);
      return View(inquiries);
    }

    [HttpPost]
    public async Task<IActionResult> SaveFeedback(int id, string buyerComment, int actionType)
    {      
        if (!string.IsNullOrEmpty(buyerComment) && id > 0)
        {
          var result = await _feedback.SaveFeedbackAsync(id, buyerComment, actionType);
          TempData["success"] = result;
        }
        // Redirect or return a view after saving the comment
        return RedirectToAction("Index", "CustomerResponse");      
    }

    [HttpGet]
    public async Task<IActionResult> CheckPendingActivities(int id)
    {
      var pendingActivities = await _feedback.CheckPendingActivities(id);
      return Json(new { hasPendingActivities = pendingActivities > 0 });      
    }

  }
}
