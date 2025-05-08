using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class ApprovalRequestController(IRespond db, ISessionHelper sessionHelper) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(ApprovalRequestController));
    private readonly IRespond _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;


    public IActionResult Index()
    {
      IEnumerable<RespondVM> trlist = _db.GetRequestList();
      _logger.Info("[ " + _iSessionHelper.GetUserName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public IActionResult Details(int Id)
    {
      RespondDetailsVM dtlist = _db.GetReqDetailsList(Id);
      _logger.Info("[ " + _iSessionHelper.GetUserName() + " ] - PAGE VISIT DETAILS");

      return View(dtlist);
    }

    [HttpPost]
    public IActionResult UpdateStatus(RespondDetailsVM oResponse, string action)
    {
      if (oResponse?.oRespondVM?.Id == null)
      {
        return View();
      }

      bool val = action == "approve";

      string[] updateRecord = _db.UpdateStatus(val, oResponse.oRespondVM.Id);

      if (updateRecord[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetUserName() + " ] - REQUEST [" + action + "]");

        TempData["Success"] = "Record updated successfully.";
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetUserName() + " ] - ERROR [" + action + "]");
        TempData["Error"] = "An error occurred while updating the record.";
      }

      // Redirect to the Index action
      return RedirectToAction("Index");
    }

  }
}
