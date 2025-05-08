using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMIS.Models.SMIS;

namespace TMIS.Models.SMIS.VM
{
    public class MachinesVM
    {
        public IEnumerable<McRented>? McRented { get; set; }
        public IEnumerable<McOwned>? McOwned { get; set; }
    }
}
