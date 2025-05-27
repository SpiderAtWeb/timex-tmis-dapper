using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Utility;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class PrintCodesController(IPrintQR db, ISessionHelper sessionHelper) : Controller
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
    public IActionResult GenerateQrPdf([FromBody] List<string> qrCodes)
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
      byte[] pdfFile = GenerateQR.GenerateQRCode(qrCodes);

      // Return the PDF file as a downloadable response
      return File(pdfFile, "application/pdf", "TMIS_QR_CODES.pdf");
    }
  }
}
