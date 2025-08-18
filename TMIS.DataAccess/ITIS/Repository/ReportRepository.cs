using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class ReportRepository(IDatabaseConnectionSys dbConnection) : IReportRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<SummaryReportData>> GetAllDeviceData()
        {
            //order by ReturnedDate desc
            string query = "select * from ITIS_VwSummary order by ReturnedDate desc;";

            var result = await _dbConnection.GetConnection().QueryAsync<SummaryReportData>(query);
            return result;

        }

        public async Task<IEnumerable<DeviceViewModel>> GetDeviceDetail()
        {            
            string query = "select * from ITIS_VwDeviceDetails;";
            var deviceDict = new Dictionary<int, DeviceViewModel>();

            var result = await _dbConnection.GetConnection().QueryAsync<DeviceViewModel, DeviceAssignmentViewModel, DeviceViewModel>(
                query,
                (device, assignment) =>
                {
                    if (!deviceDict.TryGetValue(device.DeviceID, out var currentDevice))
                    {
                        currentDevice = device;
                        currentDevice.Assignments = new List<DeviceAssignmentViewModel>();
                        deviceDict.Add(device.DeviceID, currentDevice);
                    }

                    if (assignment != null && !string.IsNullOrEmpty(assignment.EmpName))
                    {
                        currentDevice.Assignments.Add(assignment);
                    }

                    return currentDevice;
                },
                splitOn: "EMPNo"  // <== this must be the first field of the 2nd class (DeviceAssignmentViewModel)
            );

            return [.. deviceDict.Values];                      
        }

        public async Task<IEnumerable<DeviceCountReport>> GetAllDevicesCount() 
        {
            string query = "select * from ITIS_VwDeviceCount;";
            var result = await _dbConnection.GetConnection().QueryAsync<DeviceCountReport>(query);
            return result;
        }
    }
}
