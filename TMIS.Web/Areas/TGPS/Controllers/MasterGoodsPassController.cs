using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.TGPS;

namespace TMIS.Areas.TGPS.Controllers
{
  [Area("TGPS")]

  public class MasterGoodsPassController(IAddressBank db, ISessionHelper sessionHelper, ITwoFieldsMDataAccess dbTwo) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(GenGoodsPassController));
    private readonly IAddressBank _db = db;
    private readonly ITwoFieldsMDataAccess _dbTwo = dbTwo;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult NewAddress()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAddresses()
    {
      var data = await _db.GetAllAsync();
      return Json(data);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAddress([FromBody] AddressModel model)
    {
      if (!ModelState.IsValid)
      {
        var errors = ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(new { success = false, errors });
      }

      try
      {
        if (model.Id == 0)
        {
          var id = await _db.InsertAsync(model);
          return Json(new { success = true, message = "Address Successfully Created", id });
        }
        else
        {
          await _db.UpdateAsync(model);
          return Json(new { success = true, message = "Address Successfully Updated" });
        }
      }
      catch (InvalidOperationException ex)
      {
        // Return a model-like error for consistency with client-side validation
        return BadRequest(new
        {
          success = false,
          errors = new Dictionary<string, string[]>
            {
                { "BusinessName", new[] { ex.Message } }
            }
        });
      }
      catch (Exception ex)
      {
        // Log the error if needed
        return StatusCode(500, new { success = false, message = "An unexpected error occurred.", detail = ex.Message });
      }
    }



    [HttpGet]
    public async Task<IActionResult> GetAddress(int id)
    {
      var address = await _db.GetByIdAsync(id);
      if (address == null)
        return NotFound();
      return Json(address);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAddress(int id)
    {
      var result = await _db.DeleteAsync(id);
      return Json(new { success = result > 0 });
    }


    public IActionResult NewUnits()
    {
      return View();

    }

    #region API CALLS - NewUnits

    [HttpGet]
    public IActionResult NewUnitsGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _dbTwo.GetList("TGPS_MasterTwoGpGoodsUOM");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult NewUnitsGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _dbTwo.InsertRecord(twoFieldsMData, "TGPS_MasterTwoGpGoodsUOM");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewUnits CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewUnits CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult NewUnitsGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _dbTwo.UpdateRecord(twoFieldsMData, "TGPS_MasterTwoGpGoodsUOM");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewUnits UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewUnits UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult NewUnitsGetDelete(int? id)
    {
      _dbTwo.DeleteRecord(id, "TGPS_MasterTwoGpGoodsUOM");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

    public IActionResult NewDrivers()
    {
      return View();

    }

    #region API CALLS - NewDrivers

    [HttpGet]
    public IActionResult NewDriversGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _dbTwo.GetList("TGPS_MasterTwoGpDrivers");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult NewDriversGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _dbTwo.InsertRecord(twoFieldsMData, "TGPS_MasterTwoGpDrivers");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewDrivers CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewDrivers CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult NewDriversGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _dbTwo.UpdateRecord(twoFieldsMData, "TGPS_MasterTwoGpDrivers");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewDrivers UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewDrivers UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult NewDriversGetDelete(int? id)
    {
      _dbTwo.DeleteRecord(id, "TGPS_MasterTwoGpDrivers");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion


    public IActionResult NewVehicles()
    {
      return View();

    }

    #region API CALLS - NewVehicles

    [HttpGet]
    public IActionResult NewVehiclesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _dbTwo.GetList("TGPS_MasterTwoGpVehicles");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult NewVehiclesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _dbTwo.InsertRecord(twoFieldsMData, "TGPS_MasterTwoGpVehicles");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewVehicles CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewVehicles CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult NewVehiclesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _dbTwo.UpdateRecord(twoFieldsMData, "TGPS_MasterTwoGpVehicles");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewVehicles UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewVehicles UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult NewVehiclespGetDelete(int? id)
    {
      _dbTwo.DeleteRecord(id, "TGPS_MasterTwoGpVehicles");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion


    public IActionResult NewIssues()
    {
      return View();

    }
    #region API CALLS - NewIssues

    [HttpGet]
    public IActionResult NewIssuesGetAll()
    {
      IEnumerable<TwoFieldsMData> fieldList = _dbTwo.GetList("TGPS_MasterTwoGpGoodsReasons");
      return Json(new { data = fieldList });
    }

    [HttpPost]
    public IActionResult NewIssuesGetInsert(TwoFieldsMData twoFieldsMData)
    {
      string[] insertResult = _dbTwo.InsertRecord(twoFieldsMData, "TGPS_MasterTwoGpGoodsReasons");

      if (insertResult[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewIssues CREATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = insertResult[1] });
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewIssues CREATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = insertResult[1] });
      }
    }

    [HttpPost]
    public IActionResult NewIssuesGetUpdate(TwoFieldsMData twoFieldsMData)
    {
      string[] updateResult = _dbTwo.UpdateRecord(twoFieldsMData, "TGPS_MasterTwoGpGoodsReasons");

      // Check the first element of the result array to determine success
      if (updateResult[0] == "1")
      {
        // Update successful
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewIssues UPDATED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = true, message = updateResult[1] });
      }
      else
      {
        // Update failed, return the error message
        _logger.Info("[ " + _iSessionHelper.GetShortName() + "] - NewIssues UPDATE FAILED -[" + twoFieldsMData.PropName + "]");

        return Json(new { success = false, message = updateResult[1] });
      }
    }

    [HttpGet]
    public IActionResult NewIssuesGetDelete(int? id)
    {
      _dbTwo.DeleteRecord(id, "TGPS_MasterTwoGpGoodsReasons");
      return Json(new { success = true, message = "Deleted Successful" });
    }
    #endregion

  }
}
