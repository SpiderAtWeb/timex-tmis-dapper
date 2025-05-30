namespace TMIS.Models.TGPS.VM
{
    public class ShowGPListVM
    {
        public int Id { get; set; }
        public string GGpReference { get; set; } = string.Empty;
        public string GpSubject { get; set; } = string.Empty;
        public string GeneratedUser { get; set; } = string.Empty;
        public string GeneratedDateTime { get; set; } = string.Empty;
        public string Attention { get; set; } = string.Empty;
        public string GGPRemarks { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public string ApprovedDateTime { get; set; } = string.Empty;
        public List<ShowGPItemVM> ShowGPItemVMList { get; set; } = [];
        public List<ShowGPRoutesVM> ShowGPRoutesList { get; set; } = [];
        public List<ShowGPListErrorsVM> ShowGPListErrorsList { get; set; } = [];

    }   


}
