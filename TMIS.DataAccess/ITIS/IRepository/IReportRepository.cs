using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.ITIS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.ITIS.IRepository
{
    public interface IReportRepository
    {
        Task<IEnumerable<SummaryReportData>> GetAllDeviceData();
        Task<IEnumerable<DeviceCountReport>> GetAllDevicesCount();
    }
}
