using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceUserRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList, ISessionHelper sessionHelper,
                            IITISLogdb iITISLogdb, ILdapService iLdapService) : IDeviceUserRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly ILdapService _ldapService = iLdapService;

        public async Task<IEnumerable<DeviceUserVM>> GetAllAsync()
        {
            string sql = @"SELECT dbo.ITIS_DeviceAssignments.EMPNo, dbo.ITIS_DeviceAssignments.EmpName,
                            dbo.ITIS_Devices.SerialNumber, dbo.ITIS_DeviceAssignments.DeviceID,
                            dbo.ITIS_DeviceAssignments.AssignmentID, 
                                            dbo.ITIS_DeviceAssignments.Designation
                        FROM dbo.ITIS_DeviceAssignments INNER JOIN
                        dbo.ITIS_Devices ON dbo.ITIS_DeviceAssignments.DeviceID = dbo.ITIS_Devices.DeviceID";

            return await _dbConnection.GetConnection().QueryAsync<DeviceUserVM>(sql);
        }

        public async Task<CreateDeviceUserVM> LoadDropDowns()
        {
            var objCreateDeviceUserVM = new CreateDeviceUserVM();

            objCreateDeviceUserVM = new CreateDeviceUserVM
            {
                LocationList = await _icommonList.LoadLocations(),
                DeviceSerialList = await _icommonList.LoadInstoreSerialList(),
               // EmployeeList = await _icommonList.LoadEmployeeList(),                
                EmployeeList = await _ldapService.GetEmployeesFromAD(),                
            };

            return objCreateDeviceUserVM;
        }
        
        public async Task<DeviceDetailVM> LoadDeviceDetail(int deviceID)
        {
            var objDeviceDetailVM = new DeviceDetailVM();

            objDeviceDetailVM = await _icommonList.LoadDeviceDetail(deviceID);

            return objDeviceDetailVM;
        }

        public async Task<bool> AddAsync(CreateDeviceUserVM obj)
        {
            const string query = @"
            INSERT INTO ITIS_DeviceAssignments
            (DeviceID,EMPNo,EmpName,Designation,AssignedBy,AssignRemarks,ApproverEMPNo,AssignStatusID,IsToUser)       
            VALUES (@DeviceID,@EMPNo,@EmpName,@Designation,@AssignedBy,@AssignRemarks,@ApproverEMPNo,@AssignStatusID,@IsToUser);
             SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    DeviceID = obj.AssignDevice.Device,
                    EMPNo = "",
                    EmpName = obj.AssignDevice.EmpNo,
                    Designation = "",
                    AssignedBy = _iSessionHelper.GetUserName().ToUpper(),
                    AssignRemarks = "",
                    ApproverEMPNo = "1306041",
                    AssignStatusID = 2,
                    IsToUser = true
                });

                if (insertedId.HasValue)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "DEVICE ASSIGNED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);
                }

                return insertedId > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
