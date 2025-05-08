using Microsoft.AspNetCore.Mvc.ModelBinding;
using TMIS.Models.PLMS;

namespace TMIS.Helper
{
  public class InquiryValidator
  {
    public static void ValidateInquiry(Inquiry inquiry, ModelStateDictionary modelState)
    {
      if (inquiry == null)
      {
        modelState.AddModelError(string.Empty, "Inquiry data is missing.");
        return;
      }

      if (inquiry.CustomerId == 0)
        modelState.AddModelError("Inquiry.CustomerId", "Customer Field is Required!");

      if (inquiry.InquiryTypeId == 0)
        modelState.AddModelError("Inquiry.InquiryTypeId", "Inquiry Type Field is Required!");

      if (inquiry.ResponseTypeId == 0)
        modelState.AddModelError("Inquiry.ResponseTypeId", "Response Type Field is Required!");

      if (inquiry.InquirySeasonId == 0)
        modelState.AddModelError("Inquiry.InquirySeasonId", "Season Field is Required!");

      if (inquiry.SampleStageId == 0)
        modelState.AddModelError("Inquiry.SampleStageId", "Sample Stage Field is Required!");

      if (inquiry.SampleTypeId == 0)
        modelState.AddModelError("Inquiry.SampleTypeId", "Sample Type Field is Required!");

      if (string.IsNullOrEmpty(inquiry.StyleNo))
        modelState.AddModelError("Inquiry.StyleNo", "Style No Field is Required!");

      if (string.IsNullOrEmpty(inquiry.StyleDesc))
        modelState.AddModelError("Inquiry.StyleDesc", "Style Desc Field is Required!");

      if (string.IsNullOrEmpty(inquiry.ColorCode))
        modelState.AddModelError("Inquiry.ColorCode", "Color Code Field is Required!");

      if (string.IsNullOrEmpty(inquiry.InquiryComment))
        modelState.AddModelError("Inquiry.InquiryComment", "Inquiry Comment Field is Required!");

      if (string.IsNullOrEmpty(inquiry.InquiryRecDate))
        modelState.AddModelError("Inquiry.InquiryRecDate", "Inquiry Received Date is Required!");

      if (string.IsNullOrEmpty(inquiry.InquiryReqDate))
        modelState.AddModelError("Inquiry.InquiryReqDate", "Inquiry Required Date is Required!");
    }
  }
}
