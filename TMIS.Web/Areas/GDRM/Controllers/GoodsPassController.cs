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
      var oDispaching = await _db.GetDispachingList();
      return View(oDispaching);
    }

    [HttpGet]
    public async Task<IActionResult> GetGatepassDetails(int id)
    {
      var gatepass = await _db.GetGatepassByIdAsync(id);
      return Json(gatepass);
    }

    [HttpPost]
    public async Task<IActionResult> Dispatching([FromBody] Dispatching dispatch)
    {
      var result = await _db.DispatchingGoods(dispatch);
      return Json(result);
    }

  }
}
