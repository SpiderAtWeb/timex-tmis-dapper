using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.GDRM
{
    public class GPGrUpdate
    {
        public int SelectedGpId { get; set; }

        public int VehicleNoId { get; set; }

        public int DriverNameId { get; set; }

        public bool ActionType { get; set; }

        public int GRId { get; set; }

        public int  IsOut { get; set; }

        public List<GPGrUpdateDetail> GPGrUpdateDetailList { get; set; } = [];
    }
}
