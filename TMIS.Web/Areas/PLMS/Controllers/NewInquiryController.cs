using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Helper;
using TMIS.Models.PLMS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class NewInquiryController(ICommon common, INewInquiry db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(NewInquiryController));
    private readonly ICommon _common = common;
    private readonly INewInquiry _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _common.GetInquiriesAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> Create()
    {
      var oInquiriesVM = await _common.LoadInquiryDropDowns();
      return View(oInquiriesVM);
    }

    [HttpPost]
    public async Task<IActionResult> Create(InquiryVM inquiryVM, IFormFile? artwork)
    {
      var oInquiriesVM = await _common.LoadInquiryDropDowns();

      if (artwork == null)
      {
        ModelState.AddModelError("artwork", "Artwork file is required.");
      }

      // Check if Inquiry data exists and validate
      if (inquiryVM.Inquiry == null)
      {
        ModelState.AddModelError(string.Empty, "Inquiry data is missing.");
        return View(oInquiriesVM);
      }

      InquiryValidator.ValidateInquiry(inquiryVM.Inquiry, ModelState);


      // Check if model state is valid
      if (!ModelState.IsValid)
      {
        // Preserve the user's input in the view model
        oInquiriesVM.Inquiry = inquiryVM.Inquiry;
        return View(oInquiriesVM);
      }

      var result = await _db.SaveMasterInquiryAsync(inquiryVM, artwork);
      TempData["Success"] = result;
      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LoadActsAndSubActs([FromBody] InquiryParams paraSelected)
    {
      var dynamicModel = await _db.LoadActsAndSubActsAsync(paraSelected);
      return Json(dynamicModel.ActivityList);
    }
  }
}
