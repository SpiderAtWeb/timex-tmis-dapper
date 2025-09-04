using Microsoft.AspNetCore.Mvc.ModelBinding;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.Helper
{
  public static class MachineValidator
  {
    public static void ValidateRentedMachine(McCreatedRnVM mcCreatedRnVM, ModelStateDictionary modelState)
    {
      // Serial Number Validation
      if (string.IsNullOrWhiteSpace(mcCreatedRnVM.McInventory!.SerialNo))
      {
        modelState.AddModelError("McInventory.SerialNo", "Serial No is Required.");
      }

      // Service Sequence Validation
      if (mcCreatedRnVM.McInventory.ServiceSeq <= 0)
      {
        modelState.AddModelError("McInventory.ServiceSeq", "Service Sequence is Required.");
      }

      // Model and Brand Validation
      if (mcCreatedRnVM.McInventory.MachineModelId == 0)
      {
        modelState.AddModelError("McInventory.MachineModelId", "Machine Model is Required.");
      }
      if (mcCreatedRnVM.McInventory.MachineBrandId == 0)
      {
        modelState.AddModelError("McInventory.MachineBrandId", "Machine Brand is Required.");
      }
      if (mcCreatedRnVM.McInventory.MachineTypeId == 0)
      {
        modelState.AddModelError("McInventory.MachineTypeId", "Machine Type is Required.");
      }

      // Other IDs Validation     
      if (mcCreatedRnVM.McInventory.OwnedUnitId == 0)
      {
        modelState.AddModelError("McInventory.OwnedUnitId", "Unit is Required.");
      }
      if (mcCreatedRnVM.McInventory.LocationId == 0)
      {
        modelState.AddModelError("McInventory.LocationId", "Location is Required.");
      }

      // Rented Specific Validation
      if (mcCreatedRnVM.McInventory.SupplierId == 0)
      {
        modelState.AddModelError("McInventory.SupplierId", "Supplier is Required.");
      }
      if (mcCreatedRnVM.McInventory.CostMethodId == 0)
      {
        modelState.AddModelError("McInventory.CostMethodId", "Cost method is Required.");
      }

      // Date Validation
      if (mcCreatedRnVM.McInventory.DateBorrow == null)
      {
        modelState.AddModelError("McInventory.DateBorrow", "Borrow Date is Required.");
      }
      else if (mcCreatedRnVM.McInventory.DateBorrow > DateTime.Now)
      {
        modelState.AddModelError("McInventory.DateBorrow", "Borrow Date Should be a Current or Past Date.");
      }

      if (mcCreatedRnVM.McInventory.DateDue == null)
      {
        modelState.AddModelError("McInventory.DateDue", "Due Date is Required.");
      }
      else if (mcCreatedRnVM.McInventory.DateDue < DateTime.Now)
      {
        modelState.AddModelError("McInventory.DateDue", "Due Date Not be a Current or Past Date.");
      }

      // Cost Validation
      if (mcCreatedRnVM.McInventory.Cost <= 0)
      {
        modelState.AddModelError("McInventory.Cost", "Borrow cost is Required.");
      }
    }

    public static void ValidateOwnedMachine(McCreateVM mcCreateVM, ModelStateDictionary modelState)
    {

      // Serial Number Validation
      if (string.IsNullOrWhiteSpace(mcCreateVM.McInventory!.SerialNo))
      {
        modelState.AddModelError("McInventory.SerialNo", "Serial No is Required.");
      }

      // Far Code Validation
      if (string.IsNullOrWhiteSpace(mcCreateVM.McInventory.FarCode))
      {
        modelState.AddModelError("McInventory.FarCode", "FarCode is Required.");
      }

      // Service Sequence Validation
      if (mcCreateVM.McInventory.ServiceSeq <= 0)
      {
        modelState.AddModelError("McInventory.ServiceSeq", "Service Sequence is Required.");
      }

      // Model and Brand Validation
      if (mcCreateVM.McInventory.MachineModelId == 0)
      {
        modelState.AddModelError("McInventory.MachineModelId", "Machine Model is Required.");
      }
      if (mcCreateVM.McInventory.MachineBrandId == 0)
      {
        modelState.AddModelError("McInventory.MachineBrandId", "Machine Brand is Required.");
      }
      if (mcCreateVM.McInventory.MachineTypeId == 0)
      {
        modelState.AddModelError("McInventory.MachineTypeId", "Machine Type is Required.");
      }

      // Other IDs Validation     
      if (mcCreateVM.McInventory.OwnedUnitId == 0)
      {
        modelState.AddModelError("McInventory.OwnedUnitId", "Unit is Required.");
      }
      if (mcCreateVM.McInventory.LocationId == 0)
      {
        modelState.AddModelError("McInventory.LocationId", "Location is Required.");
      }

      // Date Validation
      if (mcCreateVM.McInventory.DatePurchased == null)
      {
        modelState.AddModelError("McInventory.DatePurchased", "Purchased Date is Required.");
      }
      else if (mcCreateVM.McInventory.DatePurchased > DateTime.Now)
      {
        modelState.AddModelError("McInventory.DatePurchased", "Purchase Date Should be a Current or Past Date.");
      }

      // Cost Validation
      if (mcCreateVM.McInventory.Cost <= 0)
      {
        modelState.AddModelError("McInventory.Cost", "Purchased Cost is Required.");
      }
    }

    public static void ValidateOwnedMachineEdit(McCreateVM mcCreateVM, ModelStateDictionary modelState)
    {

      // Serial Number Validation
      if (string.IsNullOrWhiteSpace(mcCreateVM.McInventory!.SerialNo))
      {
        modelState.AddModelError("McInventory.SerialNo", "Serial No is Required.");
      }

      // Far Code Validation
      if (string.IsNullOrWhiteSpace(mcCreateVM.McInventory.FarCode))
      {
        modelState.AddModelError("McInventory.FarCode", "FarCode is Required.");
      }

      // Service Sequence Validation
      if (mcCreateVM.McInventory.ServiceSeq <= 0)
      {
        modelState.AddModelError("McInventory.ServiceSeq", "Service Sequence is Required.");
      }

      // Model and Brand Validation
      if (mcCreateVM.McInventory.MachineModelId == 0)
      {
        modelState.AddModelError("McInventory.MachineModelId", "Machine Model is Required.");
      }
      if (mcCreateVM.McInventory.MachineBrandId == 0)
      {
        modelState.AddModelError("McInventory.MachineBrandId", "Machine Brand is Required.");
      }
      if (mcCreateVM.McInventory.MachineTypeId == 0)
      {
        modelState.AddModelError("McInventory.MachineTypeId", "Machine Type is Required.");
      }


      // Date Validation
      if (mcCreateVM.McInventory.DatePurchased == null)
      {
        modelState.AddModelError("McInventory.DatePurchased", "Purchased Date is Required.");
      }
      else if (mcCreateVM.McInventory.DatePurchased > DateTime.Now)
      {
        modelState.AddModelError("McInventory.DatePurchased", "Purchase Date Should be a Current or Past Date.");
      }

      // Cost Validation
      if (mcCreateVM.McInventory.Cost <= 0)
      {
        modelState.AddModelError("McInventory.Cost", "Purchased Cost is Required.");
      }
    }

    public static void ValidateTrasnferMachine(McRequestDetailsVM mcCreateVM, ModelStateDictionary modelState)
    {
      if (mcCreateVM?.ReqUnitId == 0)
      {
        modelState.AddModelError("ReqUnitId", "Request Unit is Required !");
      }

      if (mcCreateVM?.ReqLocId == 0)
      {
        modelState.AddModelError("ReqLocId", "Request Location is Required !");
      }

      if (mcCreateVM?.ReqRemark == null)
      {
        modelState.AddModelError("ReqRemark", "Remarks is Required !");
      }     
    }

  }

}
