using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.GDRM
{
    public class Dispatching
    {
        [Required]
        public int SlectedGpId { get; set; }

        [Required]
        public int SlectedGpIdType { get; set; }

        [Required]
        public int VehicleNoId { get; set; }

        [Required]
        public int DriverNameId { get; set; }

        [ValidateNever]
        public List<DispatchingDetail> DispatchingDetails { get; set; } = [];
    }
}
