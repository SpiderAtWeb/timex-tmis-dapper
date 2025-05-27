using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
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
                        dbo.ITIS_Devices ON dbo.ITIS_DeviceAssignments.DeviceID = dbo.ITIS_Devices.DeviceID
                        where dbo.ITIS_DeviceAssignments.AssignStatusID not in (4,5)";

            return await _dbConnection.GetConnection().QueryAsync<DeviceUserVM>(sql);
        }

        public async Task<CreateDeviceUserVM> LoadDropDowns()
        {
            var objCreateDeviceUserVM = new CreateDeviceUserVM();

            objCreateDeviceUserVM = new CreateDeviceUserVM
            {
                LocationList = await _icommonList.LoadLocations(),
                DepartmentList = await _icommonList.LoadDepartments(),
                DeviceSerialList = await _icommonList.LoadInstoreSerialList(),
                ApproverList = await _icommonList.LoadApproverList(),
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

        public async Task<bool> ReturnDevice(ReturnDeviceVM obj, IFormFile? image)
        {
            const string query = @"update ITIS_DeviceAssignments set AssignStatusID=5, ReturnedDate=GETDATE(), ReturnRemarks=@ReturnRemarks, 
                         ReturnTimeImage= @ReturnTimeImag where AssignmentID=@AssignmentID";

            try
            {
                byte[]? imageBytes = null;


                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    ReturnRemarks = obj.ReturnRemark,
                    AssignmentID = obj.RecordID,
                    ReturnTimeImag = imageBytes
                });

                if (rowsAffected > 0) 
                {
                    const string deviceQuery = @"update ITIS_DEVICES set DeviceStatusID=6 where DeviceID=@DeviceID";

                    _dbConnection.GetConnection().Execute(deviceQuery, new
                    {
                        DeviceID = obj.Device
                    });

                    Logdb logdb = new()
                    {
                        TrObjectId = obj.RecordID,
                        TrLog = "DEVICE RETURNED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);
                }

                return true;

            }
            catch (Exception ex)
            { 
                return false;
            }
        }


        public async Task<bool> AddAsync(CreateDeviceUserVM obj)
        {
            const string query = @"
            INSERT INTO ITIS_DeviceAssignments
            (DeviceID,EMPNo,EmpName,Designation,AssignedBy,AssignRemarks,ApproverEMPNo,AssignStatusID,IsToUser, AssignLocation, AssignDepartment)       
            VALUES (@DeviceID,@EMPNo,@EmpName,@Designation,@AssignedBy,@AssignRemarks,@ApproverEMPNo,@AssignStatusID,@IsToUser,@AssignLocation,@AssignDepartment);
             SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    DeviceID = obj.AssignDevice.Device,
                    EMPNo = "",
                    EmpName = obj.AssignDevice.EmpNo,
                    Designation = obj.AssignDevice.Designation,
                    AssignedBy = _iSessionHelper.GetUserName().ToUpper(),
                    AssignRemarks = "",
                    ApproverEMPNo = obj.AssignDevice.Approver, // add actual approver empno
                    AssignStatusID = 2,
                    IsToUser = true,
                    AssignLocation = obj.AssignDevice.AssignLocation,
                    AssignDepartment = obj.AssignDevice.AssignDepartment
                });

                if (insertedId.HasValue)
                {
                    const string deviceQuery = @"update ITIS_DEVICES set DeviceStatusID=7 where DeviceID=@DeviceID";

                    _dbConnection.GetConnection().Execute(deviceQuery, new
                    {
                        DeviceID = obj.AssignDevice.Device
                    });

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


        public async Task<ReturnDeviceVM> LoadInUseDevices()
        {
            var objReturnDeviceVM = new ReturnDeviceVM();

            objReturnDeviceVM = new ReturnDeviceVM
            {            
                DeviceSerialList = await _icommonList.LoadInUseSerialList()                              
            };

            return objReturnDeviceVM;
        }

        public async Task<DeviceUserDetailVM> LoadUserDetail(int deviceID)
        {
            var objDeviceUserDetailVM = new DeviceUserDetailVM();

            objDeviceUserDetailVM = await _icommonList.LoadUserDetail(deviceID);

            return objDeviceUserDetailVM;
        }
    }
}
