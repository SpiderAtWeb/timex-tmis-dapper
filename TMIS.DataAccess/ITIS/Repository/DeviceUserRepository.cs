using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TAPS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceUserRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList, ISessionHelper sessionHelper,
                            IITISLogdb iITISLogdb, IGmailSender gmailSender) : IDeviceUserRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IGmailSender _gmailSender = gmailSender;

        public async Task<IEnumerable<DeviceUserVM>> GetAllAsync()
        {
            string sql = @"select a.AssignmentID, a.DeviceID, t.DeviceType, d.DeviceName, d.SerialNumber, ISNull(ad.EmpName, a.EmpName) as EmpName, a.Designation, 
                            a.AssignedDate, a.AssignLocation as LocationName, a.AssignDepartment as DepartmentName, st.PropName as AssignStatus 
                            from ITIS_DeviceAssignments as a 
                            left join ITIS_MasterADEmployees as ad on ad.EmpUserName=a.EmpName
                            left join ITIS_Devices as d on d.DeviceID=a.DeviceID
                            left join ITIS_DeviceTypes as t on t.DeviceTypeID=d.DeviceTypeID                  
                            left join ITIS_DeviceAssignStatus as st on st.Id=a.AssignStatusID
                            where a.AssignStatusID not in (4, 5)";

            return await _dbConnection.GetConnection().QueryAsync<DeviceUserVM>(sql);
        }

        public async Task<CreateDeviceUserVM> LoadDropDowns()
        {
            var objCreateDeviceUserVM = new CreateDeviceUserVM();

            objCreateDeviceUserVM = new CreateDeviceUserVM
            {
                LocationList = await _icommonList.LoadLocationsFromAD(),
                DepartmentList = await _icommonList.LoadDepartmentsFromAD(),
                DeviceSerialList = await _icommonList.LoadInstoreSerialList(),
                ApproverList = await _icommonList.LoadApproverList(),
                EmployeeList = await _icommonList.LoadEmployeeList(),
                DesignationList = await _icommonList.LoadDesignationsFromAD(),
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
            catch
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
                    DeviceID = obj.AssignDevice!.Device,
                    EMPNo = obj.AssignDevice!.EmpNo,
                    EmpName = obj.AssignDevice.EmpName,
                    Designation = obj.AssignDevice.Designation,
                    AssignedBy = _iSessionHelper.GetShortName().ToUpper(),
                    AssignRemarks = obj.AssignDevice.AssignRemark,
                    ApproverEMPNo = obj.AssignDevice.Approver,
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
        public async Task<DeviceUserDetailVM?> SelectedEmpValues(string empName)
        {
            var objDeviceUserDetailVM = new DeviceUserDetailVM();

            string sql = @"select EmpDesignation as Designation, EmpLocation as AssignLocation, EmpDepartment as AssignDepartment from ITIS_MasterADEMPLOYEES where EmpUserName=@EmpName";
            
            objDeviceUserDetailVM = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceUserDetailVM>(sql, new
            {
                EmpName = empName
            });
            return objDeviceUserDetailVM;
        }

        private void PrepairEmail(int genId)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"SELECT  EmpGpNo, GateName, ExpLoc, ExpReason, ExpOutTime, IsReturn, GenUser, ApprovedById
            FROM TGPS_VwEGPHeaders WHERE (Id = @GenId)";

            var header = connection.Query(headerQuery, new { GenId = genId }).FirstOrDefault();

            string detailsQuery = @"SELECT EmpName, EmpEPF
            FROM            TGPS_VwEGPDetails WHERE        (EGpPassId = @GenId)";

            var details = connection.Query(detailsQuery, new { GenId = genId }).ToList();

            // Prepare header part of array
            var myList = new List<string>
            {
                $"{header!.EmpGpNo}",
                $"{header.GateName}",
                $"{header.ExpLoc}",
                $"{header.ExpReason}",
                $"{header.ExpOutTime}",
                $"{header.IsReturn}",
                $"{header.GenUser}"
            };

            // Append each detail row as a string item in the array
            foreach (var d in details)
            {
                string detailString = $"{d.EmpName}|{d.EmpEPF}";
                myList.Add(detailString);
            }

            // Convert to array
            string[] myArray = [.. myList];

            // Prepare email recipient
            string mailTo = connection.QuerySingleOrDefault<string>(
                "SELECT UserEmail FROM ADMIN.dbo._MasterUsers WHERE Id = @Id",
                new { Id = header.ApprovedById }
            ) ?? throw new InvalidOperationException("No email found for the approved user.");

            // Send email
            Task.Run(() => _gmailSender.EPRequestToApprove(mailTo, myArray));
        }

    }
}
