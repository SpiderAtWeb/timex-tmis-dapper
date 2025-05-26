using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.Models.GDRM;
using TMIS.Models.GDRM.VM;

namespace TMIS.Areas.GDRM.Controllers
{
  [Area("GDRM")]
  public class GoodsPassController(IGRGoods db) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(GoodsPassController));
    private readonly IGRGoods _db = db;

    public async Task<IActionResult> Index()
    {
      var oPedings = await _db.GetPendingList();
      return View(oPedings);
    }

    [HttpGet]
    public async Task<IActionResult> GetGatepassDetails(int id, bool isOut)
    {
      var gatepass = await _db.GetGatepassByIdAsync(id, isOut);
      return Json(gatepass);
    }

    [HttpPost]
    public async Task<IActionResult> GPUpdate([FromBody] GPGrUpdate gPGrUpdate)
    {
      var result = await _db.GatePassUpdating(gPGrUpdate);
      return Json(result);
    }

  }
}
