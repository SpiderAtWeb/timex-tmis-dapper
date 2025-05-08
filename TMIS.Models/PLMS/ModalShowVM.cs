namespace TMIS.Models.PLMS
{
    public class ModalShowVM
    {
        public string[] LogStrings { get; set; }
        public byte[]? ArtWork { get; set; }
        public List<PLMSTrActivity>? ActivityList { get; set; }

        public ModalShowVM()
        {
            LogStrings = [];
            ActivityList = [];
        }
    }
}
