using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.Areas.SMIS.Controllers
{
  [Authorize(Roles = "admin")]
  [Area("SMIS")]
  public class MasterRentingController(ITwoFieldsMDataAccess db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(MasterRentingController));
    private readonly ITwoFieldsMDataAccess _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View();
    }

    public IActionResult RentSuppliers()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT SUPPLIERS");

      return View();
    }

    public IActionResult CostMethods()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT COST-METHODS");

      return View();
    }

    #region API CALLS - Renting Supplier

    [HttpGet]
    public IActionResult RentSupGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MdMachineSuppliers");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult RentSupGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MdMachineSuppliers");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - RentSup CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - RentSup CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult RentSupGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MdMachineSuppliers");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - RentSup UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - RentSup UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult RentSupGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MdMachineSuppliers");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Cost Methods

    [HttpGet]
    public IActionResult CostGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MdCostMethods");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult CostGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MdCostMethods");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Cost CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Cost CREATE FALIED-[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult CostGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MdCostMethods");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Cost UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Cost UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult CostGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MdCostMethods");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

  }
}
