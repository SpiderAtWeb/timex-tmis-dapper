namespace TMIS.Models.GDRM
{
    public class EmpGpUpdateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorFieldId { get; set; } = string.Empty;
    }
}
