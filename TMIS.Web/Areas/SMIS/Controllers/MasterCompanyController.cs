using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.Areas.SMIS.Controllers
{
  [Authorize(Roles = "SUPER-ADMIN")]
  [Area("SMIS")]
  public class MasterCompanyController(ITwoFieldsMDataAccess db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ITwoFieldsMDataAccess _db = db;
    private readonly ILog _logger = LogManager.GetLogger(typeof(MasterCompanyController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Locations()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT LOCATION");

      return View();
    }

    public IActionResult Clusters()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT CLUSTER");

      return View();
    }

    public IActionResult Units()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT UNITS");

      return View();
    }

    public IActionResult Groups()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT GROUPS");

      return View();
    }

    #region API CALLS - Company Group

    [HttpGet]
    public IActionResult GroupGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("COMN_MasterTwoGroups");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult GroupGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "COMN_MasterTwoGroups");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Group CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Group CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult GroupGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "COMN_MasterTwoGroups");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("Group UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("Group UPDATE FAILED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult GroupGetDelete(int? id)
    {
      _db.DeleteRecord(id, "COMN_MasterTwoGroups");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Company Locations

    [HttpGet]
    public IActionResult LocationsGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MasterTwoSewingLines");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult LocationsGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MasterTwoSewingLines");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Locations CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Locations CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult LocationsGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MasterTwoSewingLines");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Locations UPDATED -[" + twoFieldsMData.PropName + "]");

        // Update successful
        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Locations UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult LocationsGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MasterTwoSewingLines");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Cluster

    [HttpGet]
    public IActionResult OwnedClusterGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("COMN_MasterTwoClusters");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult OwnedClusterGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "COMN_MasterTwoClusters");

      if (insertResult[0] == "1")
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - OwnedCluster CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - OwnedCluster CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult OwnedClusterGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "COMN_MasterTwoClusters");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("OwnedCluster UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update successful
        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("OwnedCluster UPDATe FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult OwnedClusterGetDelete(int? id)
    {
      _db.DeleteRecord(id, "COMN_MasterTwoClusters");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Unit

    [HttpGet]
    public IActionResult OwnedUnitGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("COMN_MasterTwoLocations");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult OwnedUnitGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "COMN_MasterTwoLocations");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - OwnedUnit CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - OwnedUnit CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult OwnedUnitGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "COMN_MasterTwoLocations");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - OwnedUnit UPDATED -[" + twoFieldsMData.PropName + "]");

        // Update successful
        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - OwnedUnit UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult OwnedUnitGetDelete(int? id)
    {
      _db.DeleteRecord(id, "COMN_MasterTwoLocations");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

  }
}
