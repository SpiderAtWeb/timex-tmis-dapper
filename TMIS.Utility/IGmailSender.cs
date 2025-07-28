using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.HRRS;

namespace TMIS.Utility
{
    public interface IGmailSender
    {
        public void McRequestToApprove(params string[] myArray);
        public void GPRequestToApprove(string mailTo, string[] myArray);
        public void EPRequestToApprove(string mailTo, string[] myArray);
        public void RequestToApprove(string mailTo, string[] myArray);
        public void ITRequestToApprove(HRRS_ITRequest hRRS_ITRequest);
        public void ITRequestToIT(HRRS_ITRequest hRRS_ITRequest);

    }
}
