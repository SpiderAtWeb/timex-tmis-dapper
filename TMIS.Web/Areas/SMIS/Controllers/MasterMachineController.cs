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
  public class MasterMachineController(ITwoFieldsMDataAccess db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ITwoFieldsMDataAccess _db = db;
    private readonly ILog _logger = LogManager.GetLogger(typeof(MasterMachineController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult MachineTypes()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT MC TYPES");

      return View();
    }

    public IActionResult MachineBrands()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT MC BRANDS");

      return View();
    }

    public IActionResult MachineModels()
    {
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT MC MODELS");

      return View();
    }



    #region API CALLS - Machine Type

    [HttpGet]
    public IActionResult TypeGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MdMachineTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult TypeGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MdMachineTypes");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Type CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Type CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult TypeGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MdMachineTypes");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Type UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Type UPDATED FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult TypeGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MdMachineTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Machine Brand

    [HttpGet]
    public IActionResult BrandsGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MdMachineBrands");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult BrandsGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MdMachineBrands");

      if (insertResult[0] == "1")
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Brands CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Brands CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult BrandsGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MdMachineBrands");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Brands UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Brands UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult BrandsGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MdMachineBrands");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Machine Model

    [HttpGet]
    public IActionResult ModelGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("SMIM_MdMachineModels");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult ModelGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "SMIM_MdMachineModels");

      if (insertResult[0] == "1")
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Model CREATED FAIL-[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Model CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult ModelGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "SMIM_MdMachineModels");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Model UPDATE FAIL-[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Model UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult ModelGetDelete(int? id)
    {
      _db.DeleteRecord(id, "SMIM_MdMachineModels");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion



  }
}
