using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.TTMS.VM
{
    public class NewTransporterViewModel
    {
        public Transporter Transporter { get; set; } = new Transporter();   
    }
}
