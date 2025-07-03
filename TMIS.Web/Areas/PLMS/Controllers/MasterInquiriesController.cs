using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;


namespace TMIS.Areas.PLMS.Controllers
{

  [Area("PLMS")]
  public class MasterInquiriesController(ITwoFieldsMDataAccess db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ITwoFieldsMDataAccess _db = db;
    private readonly ILog _logger = LogManager.GetLogger(typeof(MasterInquiriesController));
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult InquiryTypes()
    {
      return View();
    }

    public IActionResult Customers()
    {
      return View();
    }

    public IActionResult SampleTypes()
    {
      return View();
    }

    public IActionResult CPActivities()
    {
      return View();
    }

    public IActionResult UserCategories()
    {
      return View();
    }

    #region API CALLS - InquiryTypes
    [HttpGet]
    public IActionResult InquiryTypesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MasterTwoInquiryTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult InquiryTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MasterTwoInquiryTypes");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - InquiryTypes CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - InquiryTypes CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult InquiryTypesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MasterTwoInquiryTypes");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("InquiryTypes UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("InquiryTypes UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult InquiryTypesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MasterTwoInquiryTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Customers
    [HttpGet]
    public IActionResult CustomersGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MasterTwoCustomers");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult CustomersGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MasterTwoCustomers");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Customers CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Customers CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult CustomersGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MasterTwoCustomers");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("Customers UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("Customers UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult CustomersGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MasterTwoCustomers");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - SampleTypes
    [HttpGet]
    public IActionResult SampleTypesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MasterTwoSampleTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult SampleTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MasterTwoSampleTypes");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - SampleTypes CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - SampleTypes CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult SampleTypesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MasterTwoSampleTypes");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("SampleTypes UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("SampleTypes UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult SampleTypesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MasterTwoSampleTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Activities
    [HttpGet]
    public IActionResult ActivitiesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MasterTwoActivityTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult ActivitiesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MasterTwoActivityTypes");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - Activities CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - Activities CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult ActivitiesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MasterTwoActivityTypes");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("Activities UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("Activities UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult ActivitiesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MasterTwoActivityTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - UserCategories
    [HttpGet]
    public IActionResult UserCategoriesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MasterTwoUserCategories");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult UserCategoriesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MasterTwoUserCategories");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - UserCategories CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - UserCategories CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult UserCategoriesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MasterTwoUserCategories");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("UserCategories UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("UserCategories UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult UserCategoriesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MasterTwoUserCategories");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion
  }
}
