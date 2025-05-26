namespace TMIS.Models.GDRM.VM
{
    public class GPNumbers
    {
        public bool IsOut { get; set; }
        public int GPId { get; set; }
        public int BaseOUT { get; set; }
        public string GPNumber { get; set; } = string.Empty;
        public string GpTo { get; set; } = string.Empty;
    }
}
