using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IRespond
    {
        public IEnumerable<RespondVM> GetRequestList();

        public RespondDetailsVM GetReqDetailsList(int Id);

        public string[] UpdateStatus(bool sts, int iD);
    }
}
