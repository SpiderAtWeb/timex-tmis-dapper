using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class DeviceController(IDeviceTypeRepository deviceTypeRepository, ISessionHelper sessionHelper) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceTypeController));
    private readonly IDeviceTypeRepository _deviceTypeRepository = deviceTypeRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT DEVICE INDEX");
      var deviceTypes = await _deviceTypeRepository.GetAllAsync();
      return View(deviceTypes);
    }
  }
}
