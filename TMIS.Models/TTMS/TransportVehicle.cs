using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class TransportVehicle
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public int DestinationId { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DestinationName { get; set; } = string.Empty;
    }
}
