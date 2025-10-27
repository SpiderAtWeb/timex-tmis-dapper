using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class VehiclePaymentAllocationViewModel
    {
        public int VehicleId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Cost { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<TransportVehicle> Vehicles { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }

        public VehiclePaymentAllocationViewModel()
        {
            Vehicles = new List<TransportVehicle>();
            PaymentMethods = new List<PaymentMethod>();
            EffectiveDate = DateTime.Today;
        }
    }
}
