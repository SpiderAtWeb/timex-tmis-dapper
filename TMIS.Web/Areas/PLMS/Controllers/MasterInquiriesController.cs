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

    public IActionResult ResponseTypes()
    {
      return View();
    }


    public IActionResult Customers()
    {
      return View();
    }


    public IActionResult SeasonTypes()
    {
      return View();
    }

    public IActionResult SampleTypes()
    {
      return View();
    }

    public IActionResult SampleStages()
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
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdInquiryTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult InquiryTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdInquiryTypes");

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
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdInquiryTypes");

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
      _db.DeleteRecord(id, "PLMS_MdInquiryTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - ResponseTypes
    [HttpGet]
    public IActionResult ResponseTypesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdReponseTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult ResponseTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdReponseTypes");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - ResponseTypes CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - ResponseTypes CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult ResponseTypesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdReponseTypes");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("ResponseTypes UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("ResponseTypes UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult ResponseTypesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MdReponseTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Customers
    [HttpGet]
    public IActionResult CustomersGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdCustomers");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult CustomersGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdCustomers");

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
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdCustomers");

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
      _db.DeleteRecord(id, "PLMS_MdCustomers");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - SeasonTypes
    [HttpGet]
    public IActionResult SeasonTypesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdInquirySeason");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult SeasonTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdInquirySeason");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - SeasonTypes CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - SeasonTypes CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult SeasonTypesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdInquirySeason");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("SeasonTypes UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("SeasonTypes UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult SeasonTypestDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MdInquirySeason");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - SampleStages
    [HttpGet]
    public IActionResult SampleStagesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdExtend");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult SampleStagesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdExtend");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - SampleStages CREATED - [" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[" + _iSessionHelper.GetShortName() + "] - SampleStages CREATE FAIL -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult SampleStagesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdExtend");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        _logger.Info("SampleStages UPDATED -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        _logger.Info("SampleStages UPDATE FAIL -[" + twoFieldsMData.PropName + "] [" + _iSessionHelper.GetShortName() + "]");

        // Update failed, return the error message
        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult SampleStagesGetDelete(int? id)
    {
      _db.DeleteRecord(id, "PLMS_MdExtend");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - SampleTypes
    [HttpGet]
    public IActionResult SampleTypesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdExtendSub");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult SampleTypesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdExtendSub");

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
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdExtendSub");

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
      _db.DeleteRecord(id, "PLMS_MdExtendSub");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - Activities
    [HttpGet]
    public IActionResult ActivitiesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdActivityTypes");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult ActivitiesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdActivityTypes");

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
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdActivityTypes");

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
      _db.DeleteRecord(id, "PLMS_MdActivityTypes");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    #region API CALLS - UserCategories
    [HttpGet]
    public IActionResult UserCategoriesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _db.GetList("PLMS_MdUserCategories");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult UserCategoriesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _db.InsertRecord(twoFieldsMData, "PLMS_MdUserCategories");

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
      string[] updateResult = _db.UpdateRecord(twoFieldsMData, "PLMS_MdUserCategories");

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
      _db.DeleteRecord(id, "PLMS_MdUserCategories");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion
  }
}
