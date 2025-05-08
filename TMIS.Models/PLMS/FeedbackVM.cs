namespace TMIS.Models.PLMS
{
    public class FeedbackVM
    {
        public ShowInquiryDataVM ShowInqDataVM { get; set; }
        public ModalShowVM ModalShowVM { get; set; }
        public InquiryVM InquiryVM { get; set; }

        public FeedbackVM()
        {
            ShowInqDataVM = new ShowInquiryDataVM();
            ModalShowVM = new ModalShowVM();
            InquiryVM = new InquiryVM();
        }

    }
}
