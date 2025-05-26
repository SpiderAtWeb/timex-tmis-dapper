using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.GDRM
{
    public class Dispatching
    {
        public int SelectedGpId { get; set; }

        public int SelectedGpIdType { get; set; }

        public int VehicleNoId { get; set; }

        public int DriverNameId { get; set; }

        public bool ActionType { get; set; }

        public List<DispatchingDetail> DispatchingDetails { get; set; } = [];
    }
}
