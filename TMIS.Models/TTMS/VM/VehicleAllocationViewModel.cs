using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class VehicleAllocationViewModel
    {
        public int VehicleId { get; set; }
        public int DestinationId { get; set; }
        public List<int> SelectedEmployeeIds { get; set; }
        public List<Employee> AvailableEmployees { get; set; }
        public List<TransportVehicle> Vehicles { get; set; }
        public List<EmployeeDestination> Destinations { get; set; }

        public VehicleAllocationViewModel()
        {
            SelectedEmployeeIds = new List<int>();
            AvailableEmployees = new List<Employee>();
            Vehicles = new List<TransportVehicle>();
            Destinations = new List<EmployeeDestination>();
        }

    }
}
