using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using TMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.DataAccess.TGPS.Rpository;
using TMIS.Models.TGPS;

namespace TMIS.Areas.TGPS.Controllers
{
  [Area("TGPS")]

  public class MasterGoodsPassController(IAddressBank db) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(GenGoodsPassController));
    private readonly IAddressBank _db = db;


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
        // Extract errors
        var errors = ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(new { success = false, errors });
      }

      if (model.Id == 0)
      {
        var id = await _db.InsertAsync(model);
        return Json(new { success = true, message = "Created", id });
      }
      else
      {
        await _db.UpdateAsync(model);
        return Json(new { success = true, message = "Updated" });
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

  }
}
