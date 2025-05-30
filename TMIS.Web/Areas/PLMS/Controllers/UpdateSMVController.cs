using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class UpdateSMVController(ISMV smv, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(UpdateSMVController));
    private readonly ISMV _smv = smv;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _smv.GetInquiriesAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> SMV(string id)
    {
      var inquiries = await _smv.GetInquiryAsync(id);
      return View(inquiries);
    }

    [HttpPost]
    public async Task<IActionResult> SaveSMV(int id, string smvValue, string smvComment)
    {
      try
      {
        var result = await _smv.SaveSMV(id, smvValue, smvComment); // Call your service method
        TempData["Success"] = result;

        return RedirectToAction("Index", "UpdateSMV");

      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
      }
    }
  }
}
