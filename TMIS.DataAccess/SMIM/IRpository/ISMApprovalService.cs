using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface ISMApprovalService
    {
        public Task<int> SMUpdateAsync(string invoiceCode, int action);

        public Task<(string Status, string InvoiceNo)> GetSMInfoAsync(string invoiceCode);

    }
}
