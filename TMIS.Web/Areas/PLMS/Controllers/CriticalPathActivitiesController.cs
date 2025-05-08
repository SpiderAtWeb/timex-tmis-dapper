using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class CriticalPathActivitiesController(ISaveCriticalPathActivity db) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(CriticalPathActivitiesController));
    private readonly ISaveCriticalPathActivity _db = db;

    public IActionResult Index()
    {
      return View();
    }

    public async Task<IActionResult> CriticalPathEditor()
    {
      var oInquiriesVM = await _db.LoadCPathDropDowns();
      return View(oInquiriesVM);
    }

    [HttpPost]
    public async Task<IActionResult> SaveTreeData([FromBody] ActivitySave activitySave)
    {

      if (activitySave == null)
        return Json(new { success = false, message = "TreeData is null" });

      await _db.SaveActivities(activitySave);
      return Json(new { success = true, message = "Tree data saved successfully!" });
    }

    [HttpPost]
    public async Task<IActionResult> LoadActivites([FromBody] InquiryParams selectedParas)
    {
      var dynamicModel = await _db.LoadSavedActivityList(selectedParas);
      return Json(dynamicModel);
    }

  }
}
