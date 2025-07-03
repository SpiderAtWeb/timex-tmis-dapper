using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.Models.PLMS;

namespace TMIS.Areas.PLMS.Controllers
{
  [Area("PLMS")]
  public class TaskCompletionController(ITaskCompletion db, ISessionHelper sessionHelper) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(TaskCompletionController));
    private readonly ITaskCompletion _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;

    public async Task<IActionResult> Index()
    {
      var inquiries = await _db.GetInquiriesUserIdAsync();
      return View(inquiries);
    }

    public async Task<IActionResult> LoadModal(string Id)
    {
      var dynamicModel = await _db.LoadModalDataUserIdAsync(Id);
      return Json(dynamicModel);

    }

    [HttpPost]
    public async Task<IActionResult> SaveCompletedTasks([FromForm] SaveTasks saveTasks)
    {
      if (saveTasks.MainTasks.Count <=0  && saveTasks.SubTasks.Count <= 0)
      {
        return BadRequest("No tasks provided.");
      }

      try
      {
        var result = await _db.SaveTasksAndSubTasksAsync(saveTasks);
        return Ok(result);
      }
      catch (Exception ex)
      {
        // Log the exception for debugging
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }


  }
}
