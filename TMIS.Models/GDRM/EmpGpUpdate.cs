namespace TMIS.Models.GDRM
{
    public class EmpGpUpdate
    {
        public int GRId { get; set; }
        public int IsOut { get; set; }
        public int SelectedEmpGpId { get; set; }
        public bool ActionType { get; set; }
        public List<EmpGpUpdateDetail> EmpGpUpdateDetailList { get; set; } = new();
    }
}
