using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class ApprovalRequestController(IRespond db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(ApprovalRequestController));
    private readonly IRespond _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;


    public IActionResult Index()
    {
      IEnumerable<RespondVM> trlist = _db.GetRequestList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public IActionResult Details(int Id)
    {
      RespondDetailsVM dtlist = _db.GetReqDetailsList(Id);
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT DETAILS");

      return View(dtlist);
    }

    [HttpPost]
    public IActionResult UpdateStatus(RespondDetailsVM oResponse, string action)
    {
      if (oResponse?.RespondVM?.Id == null)
      {
        return View();
      }

      bool val = action == "approve";

      string[] updateRecord = _db.UpdateStatus(val, oResponse.RespondVM.Id);

      if (updateRecord[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - REQUEST [" + action + "]");

        TempData["success"] = "Record updated successfully.";
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - ERROR [" + action + "]");
        TempData["Error"] = "An error occurred while updating the record.";
      }

      // Redirect to the Index action
      return RedirectToAction("Index");
    }

  }
}
