using System.Net.NetworkInformation;
using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.ITIS.Controllers
{
  [Area("ITIS")]
  public class AttributeController(IAttributeRepository attributeRepository, ISessionHelper sessionHelper) : Controller
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(AttributeController));
    private readonly IAttributeRepository _attributeRepository = attributeRepository;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    public async Task<IActionResult> Index()
    {
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT ATTRIBUTE INDEX");
      var attribute = await _attributeRepository.GetAllAsync();
      return View(attribute);
    }

    public async Task<IActionResult> Create()
    {
      var createAttributeVM = await _attributeRepository.LoadDropDowns();
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT ATTRIBUTE CREATE");
      return View(createAttributeVM);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAttributeVM obj)
    {
      // Load the necessary lists before validation
      var createAttributeVM = await _attributeRepository.LoadDropDowns();

      if(obj.Attribute.Name != null && obj.Attribute.DeviceTypeID != null)
      {
        if (await _attributeRepository.CheckAttributeExist(obj.Attribute.Name, obj.Attribute.DeviceTypeID))
        {
          ModelState.AddModelError("Attribute.Name", "Label Already Available !");
        }
      }
     
      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(createAttributeVM);
      }

      // Insert attribute if everything is valid
      await _attributeRepository.AddAsync(obj.Attribute, obj.AttributeListOption);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("ATTRIBUTE CREATED [" + obj.Attribute.Name + "] - [" + _iSessionHelper.GetUserName() + "]");

      return RedirectToAction("Index");
    }
  }
}
