using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.SMIS
{
    public class TwoFieldsMData
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? PropName { get; set; }

        [StringLength(50)]
        public string? PropDesc { get; set; }
    }
}
