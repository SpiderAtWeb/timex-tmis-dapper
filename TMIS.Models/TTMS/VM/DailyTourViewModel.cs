using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class DailyTourViewModel
    {
        public int VehicleId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<TransportVehicle> Vehicles { get; set; }
        public List<EmployeeTour> Employees { get; set; }
        public List<DateTime> MonthDays { get; set; }

        public DailyTourViewModel()
        {
            Vehicles = new List<TransportVehicle>();
            Employees = new List<EmployeeTour>();
            MonthDays = new List<DateTime>();
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
        }
    }
}

