using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS
{
    public class VehiclePaymentMethod
    {
        public int VehiclePaymentId { get; set; }
        public int VehicleId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Cost { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; } = string.Empty;
    }
}
