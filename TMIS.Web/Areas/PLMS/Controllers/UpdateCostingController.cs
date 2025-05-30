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
  public class UpdatePriceController(ICosting costing, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(UpdatePriceController));
    private readonly ICosting _costing = costing;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _costing.GetInquiriesAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> Costing(string id)
    {
      var inquiries = await _costing.GetInquiryAsync(id);
      return View(inquiries);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCosting(int id, string costPrice, string priceComment, string fob)
    {
      try
      {
        var result = await _costing.SaveCosting(id, costPrice, priceComment, fob); // Call your service method
        TempData["Success"] = result;

        return RedirectToAction("Index", "UpdatePrice");

      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
      }
    }
  }
}
