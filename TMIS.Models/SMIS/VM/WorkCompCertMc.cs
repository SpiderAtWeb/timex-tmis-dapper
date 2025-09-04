namespace TMIS.Models.SMIS.VM
{
    public class WorkCompCertMc
    {
        public int Id { get; set; }
        public string MachineSerialNo { get; set; } = string.Empty;
        public string MachineType { get; set; } = string.Empty;        
        public string MachineName { get; set; } = string.Empty;
        public string PerDayCost { get; set; } = string.Empty;

    }


}
