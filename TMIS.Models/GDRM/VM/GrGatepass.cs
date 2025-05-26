namespace TMIS.Models.GDRM.VM
{
    public class GrGatepass
    {
        public int ID { get; set; }
        public string GpSubject { get; set; } = string.Empty;
        public string GeneratedBy { get; set; } = string.Empty;
        public string Attention { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public List<GrGatepassDetails> grGatepassDetails { get; set; } = [];
    }
}
