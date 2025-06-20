using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class CPTemplateController(ISaveCriticalPathActivity db) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(CPTemplateController));
    private readonly ISaveCriticalPathActivity _db = db;

    public async Task<IActionResult> CPCreate()
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
    public async Task<IActionResult> LoadActivites([FromBody] int id)
    {
      var dynamicModel = await _db.LoadSavedActivityList(id);
      return Json(dynamicModel);
    }

  }
}
