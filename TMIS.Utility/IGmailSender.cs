using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Utility
{
    public interface IGmailSender
    {
        public void McRequestToApprove(params string[] myArray);
        public void GPRequestToApprove(string mailTo, string[] myArray);
        public void EPRequestToApprove(string mailTo, string[] myArray);
        public void RequestToApprove(string mailTo, string[] myArray);

    }
}
