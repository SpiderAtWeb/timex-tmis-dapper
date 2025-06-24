using Microsoft.AspNetCore.Mvc.ModelBinding;
using TMIS.Models.PLMS;

namespace TMIS.Helper
{
  public class InquiryValidator
  {
    public static void ValidateInquiry(NewInquiryVM inquiry, ModelStateDictionary modelState)
    {
      if (inquiry == null)
      {
        modelState.AddModelError(string.Empty, "Inquiry data is missing.");
        return;
      }

      if (inquiry.CustomerId == 0)
        modelState.AddModelError("CustomerId", "Customer Field is Required!");

      if (inquiry.InquiryTypeId == 0)
        modelState.AddModelError("InquiryTypeId", "Inquiry Type Field is Required!");

      if (inquiry.SeasonsId == 0)
        modelState.AddModelError("SeasonsId", "Season Field is Required!");

      if (inquiry.SampleTypeId == 0)
        modelState.AddModelError("SampleTypeId", "Sample Type Field is Required!");

      if (string.IsNullOrEmpty(inquiry.StyleNo))
        modelState.AddModelError("StyleNo", "Style No Field is Required!");

      if (string.IsNullOrEmpty(inquiry.StyleDesc))
        modelState.AddModelError("StyleDesc", "Style Desc Field is Required!");

      if (string.IsNullOrEmpty(inquiry.ColorCode))
        modelState.AddModelError("ColorCode", "Color Code Field is Required!");

      if (string.IsNullOrEmpty(inquiry.Remarks))
        modelState.AddModelError("Remarks", "Inquiry Comment Field is Required!");

      if (string.IsNullOrEmpty(inquiry.ReceivedDate))
        modelState.AddModelError("ReceivedDate", "Inquiry Received Date is Required!");

    }
  }
}
