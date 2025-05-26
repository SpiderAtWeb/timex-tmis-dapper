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
      var createAttributeVM = await _attributeRepository.LoadDropDowns(null);
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT ATTRIBUTE CREATE");
      return View(createAttributeVM);
    }

    public async Task<IActionResult> Edit(int id)
    {      
      var attributeDetails = await _attributeRepository.LoadDropDowns(id);
      _logger.Info("[" + _iSessionHelper.GetUserName() + "] - PAGE VISIT ATTRIBUTE EDIT");
      return View(attributeDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CreateAttributeVM obj)
    {
      var attributeDetails = await _attributeRepository.LoadDropDowns(obj.Attribute!.AttributeID);

      if (obj.Attribute.Name != null && obj.Attribute.DeviceTypeID != null)
      {
        if (await _attributeRepository.CheckAttributeExist(obj))
        {
          ModelState.AddModelError("Attribute.Name", "Label Already Available !");
        }
      }

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(attributeDetails);
      }

      // Update attribute if everything is valid
      await _attributeRepository.UpdateAttribute(obj.Attribute, obj.AttributeListOption);

      // Show success message and redirect
      TempData["success"] = "Record Updated Successfully";

      _logger.Info("ATTRIBUTE UPDATED [" + obj.Attribute.AttributeID + "] - [" + _iSessionHelper.GetUserName() + "]");

      return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAttributeVM obj)
    {
      // Load the necessary lists before validation
      var createAttributeVM = await _attributeRepository.LoadDropDowns(null);

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

      _logger.Info("ATTRIBUTE CREATED [" + obj.Attribute.AttributeID + "] - [" + _iSessionHelper.GetUserName() + "]");

      return RedirectToAction("Index");
    }
  }
}
