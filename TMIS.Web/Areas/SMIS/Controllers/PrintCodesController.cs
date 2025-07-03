using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class PrintCodesController(IPrintQR db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(PrintCodesController));
    private readonly IPrintQR _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public IActionResult Index()
    {
      var qrCodes = _db.GetQrCode().Result;
      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - PAGE VISIT INDEX");

      return View(qrCodes);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateQrPdf([FromBody] List<string> qrCodes)
    {
      if (qrCodes == null || qrCodes.Count == 0)
      {
        return BadRequest("No QR codes provided.");
      }

      string qrList = string.Empty;
      foreach (var item in qrCodes)
      {
        qrList += item + ",";
      }

      _logger.Info("[ " + _iSessionHelper.GetShortName() + " ] - [" + qrList + "] QR PRINTS");

      // Generate the PDF using the utility
      byte[] pdfFile = await _db.GetQrCodesPrint(qrCodes);

      // Return the PDF file as a downloadable response
      return File(pdfFile, "application/pdf", "TMIS_QR_CODES.pdf");
    }
  }
}
