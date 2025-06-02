using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMIS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GatepassController(ILogger<GatepassController> logger) : ControllerBase
  {
    private readonly ILogger<GatepassController> _logger = logger;

    [HttpGet("approve")]
    public Task<ActionResult> ApproveGatepass([FromBody] string request)
    {
      try
      {
        //if (!ModelState.IsValid)
        //{
        //  return BadRequest(ModelState);
        //}

        //// Validate GP Code exists
        //var gatepass = await _gatepassService.GetGatepassByCodeAsync(request.GPCode);
        //if (gatepass == null)
        //{
        //  return NotFound($"Gatepass with code {request.GPCode} not found");
        //}

        //if (gatepass.Status != "Pending")
        //{
        //  return BadRequest($"Gatepass {request.GPCode} has already been {gatepass.Status.ToLower()}");
        //}

        //// Update gatepass status
        //var result = await _gatepassService.UpdateGatepassStatusAsync(
        //    request.GPCode,
        //    "Approved",
        //    request.ApprovedBy ?? "System",
        //    request.Remarks);

        //if (result)
        //{
        //  // Send confirmation email
        //  await _emailService.SendApprovalConfirmationAsync(gatepass, "Approved", request.Remarks);

        //  _logger.LogInformation($"Gatepass {request.GPCode} approved successfully");

        //  return Ok(new GatepassApprovalResponse
        //  {
        //    Success = true,
        //    Message = "Gatepass approved successfully",
        //    GPCode = request.GPCode,
        //    Status = "Approved",
        //    ProcessedAt = DateTime.Now
        //  });
        //}

        return Task.FromResult<ActionResult>(StatusCode(500, "Failed to approve gatepass"));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error approving gatepass {request}");
        return Task.FromResult<ActionResult>(StatusCode(500, "Internal server error"));
      }
    }
  }
}
