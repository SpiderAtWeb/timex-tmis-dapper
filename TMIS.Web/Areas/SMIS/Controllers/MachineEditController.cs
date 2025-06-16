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
  public class MachineEditController(IInventory db, ISessionHelper sessionHelper, IUserControls userControls) : BaseController
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(MachineEditController));
    private readonly IInventory _db = db;
    private readonly ISessionHelper _iSessionHelper = sessionHelper;
    private readonly IUserControls _userControls = userControls;

    public async Task<IActionResult> Index()
    {
      MachinesVM machinesVM = await _db.GetList();
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT INDEX");

      return View(machinesVM);
    }

    public async Task<IActionResult> OwnedEdit(int id)
    {
      var mcEditVM = await _db.LoadInventoryDropDowns(id);

      if (mcEditVM.McInventory == null)
      {
        return NotFound();
      }

      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT OW-MC EDIT [" + mcEditVM.McInventory.SerialNo + "]");
      return View(mcEditVM);
    }

    [HttpPost]
    public async Task<IActionResult> OwnedEdit(McCreateVM mcCreateVM, IFormFile? imageFR, IFormFile? imageBK)
    {

      // Load the necessary lists before validation
      await _db.LoadOwnedMachineListsAsync(mcCreateVM);

      // Perform validations
      MachineValidator.ValidateOwnedMachine(mcCreateVM, ModelState);

      if (mcCreateVM.McInventory != null && mcCreateVM.McInventory.QrCode != null)
      {
        if (mcCreateVM.McInventory.QrCode.Length <= 7)
        {
          ModelState.AddModelError("McInventory.QrCode", "Invalid Character Count!");
        }

        if (!mcCreateVM.McInventory.QrCode.Contains("TSM"))
        {
          ModelState.AddModelError("McInventory.QrCode", "QR Prefix TSM Not Found!");
        }
      }

      if (!ModelState.IsValid)
      {
        return View(mcCreateVM);
      }

      try
      {
        int msg = await _db.UpdateOwnedMachineAsync(mcCreateVM.McInventory!, imageFR, imageBK);
        if (msg == 1)
        {
          TempData["success"] = "Record Updated successfully";
          _logger.Info("[" + _iSessionHelper.GetShortName() + "] - OW-MC EDITED [" + mcCreateVM.McInventory!.SerialNo + "]");
          return RedirectToAction(nameof(Index));
        }
        else if (msg == 2)
        {
          // If the update fails (e.g., QR code duplicate), handle it
          ModelState.AddModelError("McInventory.QrCode", "This QR Code is Already Assigned to Another Machine.");
          _logger.Info("[" + _iSessionHelper.GetShortName() + "] - This QR Code is Already Assigned to Another Machine [" + mcCreateVM.McInventory!.SerialNo + "]");

          return View(mcCreateVM);
        }
        else
        {
          // If the update fails (e.g., QR code duplicate), handle it
          ModelState.AddModelError("McInventory.SerialNo", "This Serial is Already Assigned to Another Machine.");
          _logger.Info("[" + _iSessionHelper.GetShortName() + "] - This Serial is Already Assigned to Another Machine [" + mcCreateVM.McInventory!.SerialNo + "]");

          return View(mcCreateVM);
        }
      }
      catch (Exception ex)
      {
        // Catch any exceptions thrown by the UpdateMachineAsync method
        ModelState.AddModelError("McInventory.QrCode", ex.Message);  // Show the custom error message
        return View(mcCreateVM);
      }
    }

    public async Task<IActionResult> RentedEdit(int id)
    {
      var mcEditdRnVM = await _db.LoadRentInventoryDropDowns(id);

      if (mcEditdRnVM.McInventory == null)
      {
        return NotFound();
      }
      _logger.Info("[" + _iSessionHelper.GetShortName() + "] - PAGE VISIT RN-MC EDIT [" + mcEditdRnVM.McInventory.SerialNo + "]");

      return View(mcEditdRnVM);
    }

    [HttpPost]
    public async Task<IActionResult> RentedEdit(McCreatedRnVM mcCreatedRnVM, IFormFile? imageFR, IFormFile? imageBK)
    {
      await _db.LoadRentedMachineListsAsync(mcCreatedRnVM);

      mcCreatedRnVM.SupplierList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoRentSuppliers");
      mcCreatedRnVM.CostMethodsList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoCostDuration");

      MachineValidator.ValidateRentedMachine(mcCreatedRnVM, ModelState);

      if (mcCreatedRnVM.McInventory != null && mcCreatedRnVM.McInventory.QrCode != null)
      {
        if (mcCreatedRnVM.McInventory.QrCode.Length <= 7)
        {
          ModelState.AddModelError("McInventory.QrCode", "Invalid Character Count!");
        }

        if (!mcCreatedRnVM.McInventory.QrCode.Contains("TSM"))
        {
          ModelState.AddModelError("McInventory.QrCode", "QR Prefix TSM Not Found!");
        }
      }

      if (!ModelState.IsValid)
      {
        return View(mcCreatedRnVM);
      }

      try
      {
        int msg = await _db.UpdateRentMachineAsync(mcCreatedRnVM.McInventory!, imageFR, imageBK);
        if (msg == 1)
        {
          TempData["success"] = "Record Updated Successfully";
          _logger.Info("[" + _iSessionHelper.GetShortName() + "] - RENTED MC EDITED [" + mcCreatedRnVM.McInventory!.SerialNo + "]");
          return RedirectToAction(nameof(Index));
        }
        else if (msg == 2)
        {
          // If the update fails (e.g., QR code duplicate), handle it
          ModelState.AddModelError("mcCreatedRnVM.QrCode", "This QR Code is Already Assigned to Another Machine.");
          return View(mcCreatedRnVM);
        }
        else
        {
          // If the update fails (e.g., QR code duplicate), handle it
          ModelState.AddModelError("mcCreatedRnVM.SerialNo", "This Serial is Already Assigned to Another Machine.");
          return View(mcCreatedRnVM);
        }
      }
      catch (Exception ex)
      {
        // Catch any exceptions thrown by the UpdateMachineAsync method
        ModelState.AddModelError("mcCreatedRnVM.QrCode", ex.Message);  // Show the custom error message
        return View(mcCreatedRnVM);
      }
    }
  }
}
