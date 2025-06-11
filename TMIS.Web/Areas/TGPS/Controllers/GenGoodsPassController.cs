using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.TGPS.VM;

namespace TMIS.Areas.TGPS.Controllers;

[Area("TGPS")]
public class GenGoodsPassController(IGoodsGatePass db, IExportPDF exportPDF) : BaseController
{

  private readonly ILog _logger = LogManager.GetLogger(typeof(GenGoodsPassController));
  private readonly IGoodsGatePass _db = db;
  private readonly IExportPDF _exportPDF = exportPDF;

  public async Task<IActionResult> Index()
  {
    var result = await _db.GetList();
    return View(result);
  }

  public async Task<IActionResult> InternalCreate()
  {
    var result = await _db.GetFillData(false);
    return View(result);
  }

  public async Task<IActionResult> ExternalCreate()
  {
    var result = await _db.GetFillData(true);
    if (result == null)
    {
      _logger.Error("Failed to load external gatepass data");
      return NotFound("External gatepass data not found.");
    }
    return View(result);
  }

  [HttpPost]
  public async Task<IActionResult> InternalCreate([FromBody] GatepassVM gatepassVM)
  {

    if (gatepassVM == null)
    {
      _logger.Error("GatepassVM is null");
      return BadRequest("Invalid data.");
    }

    if (gatepassVM.GatepassAddresses == null || gatepassVM.Items == null ||
        gatepassVM.GatepassAddresses.Count == 0 || gatepassVM.Items.Count == 0)
    {
      _logger.Error("Gatepass addresses or items are null/empty");
      return BadRequest("Addresses and items are required.");
    }

    if (string.IsNullOrEmpty(gatepassVM.Attention))
    {
      _logger.Error("Attention is empty");
      return BadRequest("Attention field is required.");
    }

    if (string.IsNullOrEmpty(gatepassVM.SendApprovalTo))
    {
      _logger.Error("SendApprovalTo is not selected");
      return BadRequest("Please select an approver.");
    }

    if (string.IsNullOrEmpty(gatepassVM.Remarks))
    {
      _logger.Error("Remarks is empty");
      return BadRequest("Remarks are required.");
    }

    if (gatepassVM.IsReturnable)
    {
      if (string.IsNullOrEmpty(gatepassVM.ReturnDays))
      {
        if (int.Parse(gatepassVM.ReturnDays) < 0 || int.Parse(gatepassVM.ReturnDays) > 366)
        {
          _logger.Error("ReturnDays is invalid");
          return BadRequest("Return days must be between 0 and 366.");
        }
      }
    }

    gatepassVM.IsExternal = false; // Set IsExternal to false for internal gatepass
    var result = await _db.GenerateGatePass(gatepassVM);
    if (result == "Error")
    {
      _logger.Error("Failed to generate gatepass");
      return BadRequest("Failed to generate gatepass.");
    }

    _logger.Info("Gatepass generated successfully");
    TempData["success"] = result + " - Gatepass generated successfully";

    return Ok(new { success = true, message = "Gatepass generated successfully" });
  }

  [HttpPost]
  public async Task<IActionResult> ExternalCreate([FromBody] GatepassVM gatepassVM)
  {

    if (gatepassVM == null)
    {
      _logger.Error("GatepassVM is null");
      return BadRequest("Invalid data.");
    }

    if (gatepassVM.GatepassAddresses == null || gatepassVM.Items == null ||
        gatepassVM.GatepassAddresses.Count == 0 || gatepassVM.Items.Count == 0)
    {
      _logger.Error("Gatepass addresses or items are null/empty");
      return BadRequest("Addresses and items are required.");
    }

    if (string.IsNullOrEmpty(gatepassVM.Attention))
    {
      _logger.Error("Attention is empty");
      return BadRequest("Attention field is required.");
    }

    if (string.IsNullOrEmpty(gatepassVM.SendApprovalTo))
    {
      _logger.Error("SendApprovalTo is not selected");
      return BadRequest("Please select an approver.");
    }

    if (string.IsNullOrEmpty(gatepassVM.Remarks))
    {
      _logger.Error("Remarks is empty");
      return BadRequest("Remarks are required.");
    }

    if (gatepassVM.IsReturnable)
    {
      if (string.IsNullOrEmpty(gatepassVM.ReturnDays))
      {
        if (int.Parse(gatepassVM.ReturnDays) < 0 || int.Parse(gatepassVM.ReturnDays) > 366)
        {
          _logger.Error("ReturnDays is invalid");
          return BadRequest("Return days must be between 0 and 366.");
        }
      }
    }
    gatepassVM.IsExternal = true;
    var result = await _db.GenerateGatePass(gatepassVM);
    if (result == "Error")
    {
      _logger.Error("Failed to generate gatepass");
      return BadRequest("Failed to generate gatepass.");
    }

    _logger.Info("Gatepass generated successfully");
    TempData["success"] = result + " - Gatepass generated successfully";

    return Ok(new { success = true, message = "Gatepass generated successfully" });
  }

  [HttpGet]
  public async Task<IActionResult> GetGatePassSteps(int id)
  {
    var result = await _db.GetHistoryData(id);
    return Json(result);
  }

  [HttpGet]
  public async Task<IActionResult> GetGatePassDetails(int id)
  {
    var result = await _db.LoadShowGPDataAsync(id);
    if (result == null)
    {
      _logger.Error($"No gatepass found with ID: {id}");
      return NotFound("Gatepass not found.");
    }
    return PartialView("_GatePassDetailsPartial", result);
  }

  [HttpGet]
  public IActionResult DownloadPdf(int id)
  {
    var (PdfBytes, Message) = _exportPDF.DownloadPdf(id);
    byte[] pdfBytes = PdfBytes;
    string gpRef = Message;
    if (pdfBytes.Length == 0)
    {
      _logger.Error($"Failed to download PDF for report ID: {id}");
      return NotFound("PDF not found.");
    }


    return File(pdfBytes, "application/pdf", $"GATEPASS-{gpRef}.pdf");
  }
}
