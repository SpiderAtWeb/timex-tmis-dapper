using log4net;
using Microsoft.AspNetCore.Mvc;
using TMIS.Controllers;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Helper;
using TMIS.Models.SMIS.VM;

namespace TMIS.Areas.SMIS.Controllers
{
  [Area("SMIS")]
  public class MachineAddController(IInventory db, ISessionHelper sessionHelper, IUserControls userControls) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(MachineAddController));
    private readonly IInventory _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IUserControls _userControls = userControls;

    public async Task<IActionResult> Index()
    {
      MachinesVM machinesVM = await _db.GetList();
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT INDEX");

      return View(machinesVM);
    }

    public async Task<IActionResult> OwnedCreate()
    {
      var mcCreateVM = await _db.LoadInventoryDropDowns(null);
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT OW CREATE");

      return View(mcCreateVM);
    }

    [HttpPost]
    public async Task<IActionResult> OwnedCreate(McCreateVM mcCreateVM, IFormFile? imageFR, IFormFile? imageBK)
    {
      // Load the necessary lists before validation
      await _db.LoadOwnedMachineListsAsync(mcCreateVM);

      // Perform validations
      MachineValidator.ValidateOwnedMachine(mcCreateVM, ModelState);

      if (await _db.CheckSnAlreadyAvailable(mcCreateVM.McInventory!.SerialNo))
      {
        ModelState.AddModelError("McInventory.SerialNo", "Serial Number Already Available !");
      }

      ModelState["McInventory.QrCode"]?.Errors.Clear();

      // Check if the ModelState is valid
      if (!ModelState.IsValid)
      {
        return View(mcCreateVM);
      }

      // Insert machine data if everything is valid
      await _db.InsertMachineAsync(mcCreateVM.McInventory, imageFR, imageBK);

      // Show success message and redirect
      TempData["success"] = "Record Created Successfully";

      _logger.Info("OWNED MC CREATED [" + mcCreateVM.McInventory.SerialNo + "] - [" + _iSessionHelper.GetShortName() + "]");

      return RedirectToAction("Index");
    }

    public async Task<IActionResult> OwnedDetails(int id)
    {
      var oMachine = await _db.GetOwnedMcById(id);

      if (oMachine == null)
      {
        return NotFound();
      }
      return View(oMachine);
    }

    public async Task<IActionResult> RentedCreate()
    {
      var mcCreatedRnVM = await _db.LoadRentInventoryDropDowns(null);

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT RN CREATE");

      return View(mcCreatedRnVM);
    }

    [HttpPost]
    public async Task<IActionResult> RentedCreate(McCreatedRnVM mcCreatedRnVM, IFormFile? imageFR, IFormFile? imageBK)
    {
      // Load the necessary lists before validation
      await _db.LoadRentedMachineListsAsync(mcCreatedRnVM);
      mcCreatedRnVM.SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoRentSuppliers");
      mcCreatedRnVM.CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoCostDuration");

      MachineValidator.ValidateRentedMachine(mcCreatedRnVM, ModelState);     

      if (await _db.CheckSnAlreadyAvailable(mcCreatedRnVM.McInventory!.SerialNo))
      {
        ModelState.AddModelError("McInventory.SerialNo", "Serial Number Already Available !");
      }

      ModelState["McInventory.QrCode"]?.Errors.Clear();


      if (!ModelState.IsValid)
      {
        return View(mcCreatedRnVM);
      }

      await _db.InsertRentMachineAsync(mcCreatedRnVM.McInventory, imageFR, imageBK);

      TempData["success"] = "Record Created Successfully";
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - RENTED MC CREATED [" + mcCreatedRnVM.McInventory.SerialNo + "]");

      return RedirectToAction("Index");
    }

    public async Task<IActionResult> RentedDetails(int id)
    {
      var oMachine = await _db.GetRentedMcById(id);

      if (oMachine == null)
      {
        return NotFound();
      }
      return View(oMachine);
    }
  }
}
