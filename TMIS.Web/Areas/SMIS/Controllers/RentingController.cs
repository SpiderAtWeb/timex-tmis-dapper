using iTextSharp.text.pdf;
using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class RentingController(IRenting db, ISessionHelper sessionHelper) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(RentingController));
    private readonly IRenting _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Approval()
    {
      IEnumerable<TransMC> trlist = await _db.GetList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Payments()
    {
      IEnumerable<TransMC> trlist = await _db.GetListPayments();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    public async Task<IActionResult> Details(int id)
    {
      var oMachine = await _db.GetRentedMcById(id);

      if (oMachine == null)
      {
        return NotFound();
      }
      return View(oMachine);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(string remarks, string id, string action)
    {
      if (id == null)
      {
        return View();
      }

      bool isApproved = action == "approve";

      string[] updateRecord = await _db.UpdateStatus(remarks, id, isApproved);

      if (updateRecord[0] == "1")
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - REQUEST [" + action + "]");

        TempData["success"] = updateRecord[1];
      }
      else
      {
        _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - ERROR [" + action + "]");
        TempData["Error"] = updateRecord[1];
      }

      // Redirect to the Index action
      return RedirectToAction("Approval");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Certificate(List<int> SelectedIds)
    {
      var certificateData = await _db.GetMachinesByIdsAsync(SelectedIds);
      return View(certificateData);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateVoucher(WorkCompCertificate workCompCertificate, IFormFile supplierInvoiceImage)
    {
      var (pdfBytes, message) = await _db.GetGenerateVoucher(workCompCertificate, supplierInvoiceImage);

      if (pdfBytes == null || pdfBytes.Length == 0)
      {
        TempData["Error"] = "Failed to generate PDF voucher.";
        return RedirectToAction("Certificate");
      }

      if (pdfBytes.Length == 0)
      {
        _logger.Error($"Failed to download PDF for report ID: {message}");
        return NotFound("PDF not found.");
      }

      var fileName = $"{message}.pdf";


      return File(pdfBytes, "application/pdf", fileName);
    }

    public async Task<IActionResult> ReadyPayments()
    {
      IEnumerable<PaymentsVM> trlist = await _db.GetPaymentReadyList();
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(trlist);
    }

    [HttpPost]
    public async Task<IActionResult> GetInvoiceDetails(int id)
    {
      var result = await _db.GetCertificateData(id);
      if (result == null)
      {
        _logger.Error($"No gatepass found with ID: {id}");
        return NotFound("Gatepass not found.");
      }
      return PartialView("_InvoiceModal", result);
    }

    public async Task<IActionResult> StarApprovalProcess(int id)
    {
      // do your approval logic here
      var result = await _db.StartStarApproval(id);

      return Json(new { success = true, message = result });
    }

    public async Task<IActionResult> DownloadCertPdf(int id)
    {
      var pdfBytes = await _db.GetVoucherPdf(id);
      if (pdfBytes == null) return NotFound();
      return File(pdfBytes, "application/pdf");
    }

    public async Task<IActionResult> DownloadSupInvoicePdf(int id)
    {
      var pdfBytes = await _db.GetSupInvoicePdf(id);
      if (pdfBytes == null) return NotFound();

      return File(pdfBytes, "application/pdf");  

    }

  }
}
