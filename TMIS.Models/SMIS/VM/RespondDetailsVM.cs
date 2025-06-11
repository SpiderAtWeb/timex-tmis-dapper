namespace TMIS.Models.SMIS.VM
{
    public class RespondDetailsVM
    {
        public RespondVM? RespondVM { get; set; }

        public MachinesData? MachinesData { get; set; }

        public RespondDetailsVM()
        {
            RespondVM = new RespondVM();
            MachinesData = new MachinesData();
        }

    }
}
