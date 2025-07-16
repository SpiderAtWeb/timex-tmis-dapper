using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.Models.ITIS.VM;
using TMIS.Utility;

namespace TMIS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EndorseController(IGatepassService gatepassService, ILogger<EndorseController> logger, IApproveRepository approveRepository) : ControllerBase
  {
    private readonly IGatepassService _gatepassService = gatepassService;
    private readonly IApproveRepository _approveRepository = approveRepository;
    private readonly ILogger<EndorseController> _logger = logger;


    #region Gatepass

    [HttpGet("gp-approve")]
    [AllowAnonymous] // This allows access without authentication
    public async Task<ActionResult> ApproveGatepass([FromQuery] string gatepassNumber, [FromQuery] string action)
    {
      var decGpCode = SecurityBox.DecryptString(gatepassNumber);

      try
      {      

        // Validate parameters
        if (string.IsNullOrEmpty(decGpCode) || string.IsNullOrEmpty(action))
        {
          return BadRequest("Missing required parameters: gatepassNumber and action");
        }

        // Validate action
        if (!action.Equals("approve") && !action.Equals("reject"))
        {
          return BadRequest("Invalid action. Use 'approve' or 'reject'");
        }

        // Check if gatepass exists
        var gatepassInfo =  _gatepassService.GetGatepassInfoAsync(decGpCode);
        if (gatepassInfo == null)
        {
          return NotFound($"Gatepass {decGpCode} not found");
        }

        // Check if already processed
        if (gatepassInfo == "Approved" || gatepassInfo == "Rejected")
        {
          var html = GenerateAlreadyProcessedHtml(decGpCode, gatepassInfo);
          return Content(html, "text/html");
        }

        // Update status
        var status = action.Equals("approve") ? 1 : 2;
        var result = await _gatepassService.GGPUpdatAsync(decGpCode, status);

        if (result > 0)
        {
          _logger.LogInformation($"Gatepass {decGpCode} has been {status} successfully via email link");

          // Return success HTML page
          var mStatus = action.Equals("approve") ? "Approved" : "Rejected";
          var successHtml = GenerateSuccessHtml(decGpCode, mStatus);
          return Content(successHtml, "text/html");
        }

        var errorHtml = GenerateErrorHtml("Failed to process the request. Please try again or contact support.");
        return Content(errorHtml, "text/html");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error processing gatepass approval for {decGpCode}");
        var errorHtml = GenerateErrorHtml($"An error occurred: {ex.Message}");
        return Content(errorHtml, "text/html");
      }
    }

    [HttpGet("emp-approve")]
    [AllowAnonymous] // This allows access without authentication
    public async Task<ActionResult> ApproveEmppass([FromQuery] string gatepassNumber, [FromQuery] string action)
    {
      var decGpCode = SecurityBox.DecryptString(gatepassNumber);

      try
      {     

        // Validate parameters
        if (string.IsNullOrEmpty(decGpCode) || string.IsNullOrEmpty(action))
        {
          return BadRequest("Missing required parameters: gatepassNumber and action");
        }

        // Validate action
        if (!action.Equals("approve") && !action.Equals("reject"))
        {
          return BadRequest("Invalid action. Use 'approve' or 'reject'");
        }

        // Check if gatepass exists
        var gatepassInfo = _gatepassService.GetEGatepassInfoAsync(decGpCode);
        if (gatepassInfo == null)
        {
          return NotFound($"Gatepass {decGpCode} not found");
        }

        // Check if already processed
        if (gatepassInfo == "Approved" || gatepassInfo == "Rejected")
        {
          var html = GenerateAlreadyProcessedHtml(decGpCode, gatepassInfo);
          return Content(html, "text/html");
        }

        // Update status
        var status = action.Equals("approve") ? 1 : 2;
        var result = await _gatepassService.EGPUpdatAsync(decGpCode, status);

        if (result > 0)
        {
          _logger.LogInformation($"Gatepass {decGpCode} has been {status} successfully via email link");

          // Return success HTML page
          var mStatus = action.Equals("approve") ? "Approved" : "Rejected";
          var successHtml = GenerateSuccessHtml(decGpCode, mStatus);
          return Content(successHtml, "text/html");
        }

        var errorHtml = GenerateErrorHtml("Failed to process the request. Please try again or contact support.");
        return Content(errorHtml, "text/html");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error processing gatepass approval for {decGpCode}");
        var errorHtml = GenerateErrorHtml($"An error occurred: {ex.Message}");
        return Content(errorHtml, "text/html");
      }
    }

    private static string GenerateSuccessHtml(string gatepassNumber, string status)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Gatepass {status}</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .success-icon {{
                    color: #4CAF50;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
                .title {{
                    color: #333;
                    font-size: 28px;
                    margin-bottom: 10px;
                }}
                .message {{
                    color: #666;
                    font-size: 16px;
                    line-height: 1.5;
                    margin-bottom: 20px;
                }}
                .gatepass-info {{
                    background: #f8f9fa;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 20px 0;
                }}
                .status-badge {{
                    display: inline-block;
                    padding: 8px 16px;
                    border-radius: 20px;
                    font-weight: bold;
                    color: white;
                    background-color: {(status == "Approved" ? "#4CAF50" : "#f44336")};
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='success-icon'>✓</div>
                <h1 class='title'>Action Completed Successfully!</h1>
                <div class='gatepass-info'>
                    <p><strong>Gatepass Number:</strong> {gatepassNumber}</p>
                    <p><strong>Status:</strong> <span class='status-badge'>{status}</span></p>
                    <p><strong>Processed Date:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                </div>
                <p class='message'>
                    The gatepass has been {status.ToLower()} successfully. 
                    The system has been updated and relevant parties will be notified.
                </p>
            </div>
        </body>
        </html>";
    }

    private static string GenerateAlreadyProcessedHtml(string gatepassNumber, string currentStatus)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Gatepass Already Processed</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #ffeaa7 0%, #fab1a0 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .warning-icon {{
                    color: #ff9800;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
                .title {{
                    color: #333;
                    font-size: 28px;
                    margin-bottom: 10px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='warning-icon'>⚠️</div>
                <h1 class='title'>Already Processed</h1>
                <p>Gatepass <strong>{gatepassNumber}</strong> has already been <strong>{currentStatus}</strong>.</p>
                <p>No further action is required.</p>
            </div>
        </body>
        </html>";
    }

    private static string GenerateErrorHtml(string errorMessage)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Error Processing Request</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #ff7675 0%, #d63031 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .error-icon {{
                    color: #e74c3c;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='error-icon'>❌</div>
                <h1>Error</h1>
                <p>{errorMessage}</p>
                <p>Please contact support if this issue continues.</p>
            </div>
        </body>
        </html>";
    }

    #endregion

    #region ITIS
    [HttpGet("device-approve")]
    [AllowAnonymous] // This allows access without authentication
    public async Task<ActionResult> Approve([FromQuery] string assignmentID, [FromQuery] string action)
    {
      var iD = SecurityBox.DecryptString(assignmentID);

      try
      {

        // Validate parameters
        if (string.IsNullOrEmpty(iD) || string.IsNullOrEmpty(action))
        {
          return BadRequest("Missing required parameters: deviceID and action");
        }

        // Validate action
        if (!action.Equals("approve") && !action.Equals("reject"))
        {
          return BadRequest("Invalid action. Use 'approve' or 'reject'");
        }

        int id = int.Parse(iD);

        // Check if gatepass exists
        ApproveVM? assignmentInfo = await _approveRepository.GetSelectedRecord(id);

        if (assignmentInfo == null)
        {
          return NotFound($"Assignment {id} not found");
        }

        // Check if already processed
        if (assignmentInfo.AssignStatusID == 3 || assignmentInfo.AssignStatusID == 4)
        {
          string Devicestatus = assignmentInfo.AssignStatusID == 3 ? "Approved" : "Rejected";
          var html = GenerateITISAlreadyProcessedHtml(iD, Devicestatus);
          return Content(html, "text/html");
        }

        // Update status
        var status = action.Equals("approve") ? 3 : 4;

        bool result = false;
        if (status == 3)
        {
          result = await _approveRepository.AddAsync(assignmentInfo);
        }
        if (status == 4)
        {
          result = await _approveRepository.Reject(assignmentInfo);
        }

        if (result)
        {
          _logger.LogInformation($"Device Assignment {id} has been {status} successfully via email link");

          // Return success HTML page
          var mStatus = action.Equals("approve") ? "Approved" : "Rejected";
          var successHtml = GenerateITISSuccessHtml(id.ToString(), mStatus);
          return Content(successHtml, "text/html");
        }

        var errorHtml = GenerateITISErrorHtml("Failed to process the request. Please try again or contact support.");
        return Content(errorHtml, "text/html");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error processing device assignment approval for {iD}");
        var errorHtml = GenerateITISErrorHtml($"An error occurred: {ex.Message}");
        return Content(errorHtml, "text/html");
      }
    }
    private static string GenerateITISAlreadyProcessedHtml(string assignmentID, string currentStatus)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Device Already Processed</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #ffeaa7 0%, #fab1a0 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .warning-icon {{
                    color: #ff9800;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
                .title {{
                    color: #333;
                    font-size: 28px;
                    margin-bottom: 10px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='warning-icon'>⚠️</div>
                <h1 class='title'>Already Processed</h1>
                <p>Device Assignement <strong>{assignmentID}</strong> has already been <strong>{currentStatus}</strong>.</p>
                <p>No further action is required.</p>
            </div>
        </body>
        </html>";
    }
    private static string GenerateITISSuccessHtml(string assignmentID, string status)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Device Assignment {status}</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #888787 0%, #1a1a1a 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .success-icon {{
                    color: #4CAF50;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
                .title {{
                    color: #333;
                    font-size: 28px;
                    margin-bottom: 10px;
                }}
                .message {{
                    color: #666;
                    font-size: 16px;
                    line-height: 1.5;
                    margin-bottom: 20px;
                }}
                .gatepass-info {{
                    background: #f8f9fa;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 20px 0;
                }}
                .status-badge {{
                    display: inline-block;
                    padding: 8px 16px;
                    border-radius: 20px;
                    font-weight: bold;
                    color: white;
                    background-color: {(status == "Approved" ? "#4CAF50" : "#f44336")};
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='success-icon'>✓</div>
                <h1 class='title'>Action Completed Successfully!</h1>
                <div class='gatepass-info'>
                    <p><strong>Assignment Number:</strong> {assignmentID}</p>
                    <p><strong>Status:</strong> <span class='status-badge'>{status}</span></p>
                    <p><strong>Processed Date:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                </div>
                <p class='message'>
                    The device assignment has been {status.ToLower()} successfully. 
                    The system has been updated and relevant parties will be notified.
                </p>
            </div>
        </body>
        </html>";
    }
    private static string GenerateITISErrorHtml(string errorMessage)
    {
      return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Error Processing Request</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: linear-gradient(135deg, #ff7675 0%, #d63031 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }}
                .container {{
                    background: white;
                    border-radius: 10px;
                    padding: 40px;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
                    text-align: center;
                    max-width: 500px;
                    width: 100%;
                }}
                .error-icon {{
                    color: #e74c3c;
                    font-size: 64px;
                    margin-bottom: 20px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='error-icon'>❌</div>
                <h1>Error</h1>
                <p>{errorMessage}</p>
                <p>Please contact support if this issue continues.</p>
            </div>
        </body>
        </html>";
    }
    #endregion
  }
}
