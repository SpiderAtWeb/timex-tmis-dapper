using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.HRRS;
using TMIS.Models.HRRS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.HRRS.Repository
{
    public class ITRequestRepository(IDatabaseConnectionSys dbConnection, IHRRSLogdb iHRRSLogdb,
         ISessionHelper sessionHelper, IGmailSender gmailSender,
         ICommonList iCommonList, IDatabaseConnectionAdm admConnection) : IITRequestRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IDatabaseConnectionAdm _admConnection = admConnection;
        private readonly IHRRSLogdb _iHRRSLogdb = iHRRSLogdb;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IGmailSender _gmailSender = gmailSender;

        public async Task<Create> LoadDropDowns()
        {
            var objCreateVM = new Create();

            objCreateVM = new Create
            {
                LocationList = await _icommonList.LoadLocationsFromAD(),
                DepartmentList = await _icommonList.LoadDepartmentsFromAD(),                                
                DesignationList = await _icommonList.LoadDesignationsFromAD(),
                EmployeeList = await _icommonList.LoadEmployeeList()
            };
            return objCreateVM;
        }
        public async Task<bool> AddAsync(Create obj)
        {
            const string query = @"
            INSERT INTO HRRS_ITRequests
            (
                FirstName, LastName, EmployeeNo, Designation, Department, Location, Company, NIC, InterviewDate, DueStartDate,
                IsReplacement, Replacement, Email, EmailGroup, Computer, ComputerLogin, SAP, WFX, NewLandline,
                ExistingLandline, Extension, SmartPhone, BasicPhone, SIM, HomeAddress, RequestDate, RequestRemark,
                Status, RequestBy
            )
            VALUES
            (
                @FirstName, @LastName, @EmployeeNo, @Designation, @Department, @Location, @Company, @NIC, @InterviewDate, @DueStartDate,
                @IsReplacement, @Replacement, @Email, @EmailGroup, @Computer, @ComputerLogin, @SAP, @WFX, @NewLandline,
                @ExistingLandline, @Extension, @SmartPhone, @BasicPhone, @SIM, @HomeAddress, GETDATE(), @RequestRemark,
                @Status, @RequestBy
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";



            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    obj.HRRS_ITRequest!.FirstName,
                    obj.HRRS_ITRequest!.LastName,
                    obj.HRRS_ITRequest!.EmployeeNo,
                    obj.HRRS_ITRequest!.Designation,
                    obj.HRRS_ITRequest!.Department,
                    obj.HRRS_ITRequest!.Location,
                    obj.HRRS_ITRequest!.Company,
                    obj.HRRS_ITRequest!.NIC,
                    obj.HRRS_ITRequest!.InterviewDate,
                    obj.HRRS_ITRequest!.DueStartDate,
                    obj.HRRS_ITRequest!.IsReplacement,
                    obj.HRRS_ITRequest!.Replacement,
                    obj.HRRS_ITRequest!.Email,
                    obj.HRRS_ITRequest!.EmailGroup,
                    obj.HRRS_ITRequest!.Computer,
                    obj.HRRS_ITRequest!.ComputerLogin,
                    obj.HRRS_ITRequest!.SAP,
                    obj.HRRS_ITRequest!.WFX,
                    obj.HRRS_ITRequest!.NewLandline,
                    obj.HRRS_ITRequest!.ExistingLandline,
                    obj.HRRS_ITRequest!.Extension,
                    obj.HRRS_ITRequest!.SmartPhone,
                    obj.HRRS_ITRequest!.BasicPhone,
                    obj.HRRS_ITRequest!.SIM,
                    obj.HRRS_ITRequest!.HomeAddress,
                    obj.HRRS_ITRequest!.RequestRemark,
                    Status = 1, // Default to 'Pending' or change based on logic
                    RequestBy = _iSessionHelper.GetShortName().ToUpper()
                });
                if (insertedId.HasValue)
                {
                    LogdbHRRS logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "IT REQUEST CREATED"

                    };

                    _iHRRSLogdb.InsertLog(_dbConnection, logdb);
                }

                int? id = insertedId;             

                return insertedId > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public void PrepairEmail(int? genId)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"select da.AssignmentID, dt.DeviceType, d.SerialNumber, da.AssignedDate, da.AssignedBy, da.AssignRemarks, da.AssignLocation, 
                                    da.AssignDepartment, da.ApproverEMPNo from ITIS_DeviceAssignments as da 
                                    inner join ITIS_Devices as d on d.DeviceID=da.DeviceID
                                    inner join itis_deviceTypes as dt  on dt.DeviceTypeID=d.DeviceTypeID
                                    where da.AssignmentID=@AssignmentID";

            var header = connection.Query(headerQuery, new { AssignmentID = genId }).FirstOrDefault();

            string detailsQuery = @"select ad.EmpName, da.EMPNo, da.Designation, ad.EmpEmail from ITIS_DeviceAssignments as da 
                                    left join ITIS_MasterADEmployees as ad on ad.EmpUserName=da.EmpName
                                    where da.AssignmentID=@AssignmentID";

            var details = connection.Query(detailsQuery, new { AssignmentID = genId }).FirstOrDefault();

            // Prepare header part of array
            var myList = new List<string>
            {
                $"{header!.AssignmentID}",
                $"{header!.DeviceType}",
                $"{header.SerialNumber}",
                $"{header.AssignedDate}",
                $"{header.AssignedBy}",
                $"{header.AssignRemarks}",
                $"{header.AssignLocation}",
                $"{header.AssignDepartment}",
                $"{details!.EmpName}",
                $"{details!.EMPNo}",
                $"{details!.EmpEmail}",
                $"{details!.Designation}"

            };

            // Convert to array
            string[] myArray = [.. myList];

            // Prepare email recipient
            string mailTo = _admConnection.GetConnection().QuerySingleOrDefault<string>(
                "SELECT UserEmail FROM _MasterUsers WHERE Id = @Id",
                new { Id = header.ApproverEMPNo }
            ) ?? throw new InvalidOperationException("No email found for the approved user.");

            // Send email
            Task.Run(() => _gmailSender.RequestToApprove(mailTo, myArray));
        }
        public async Task<IEnumerable<HRRS_ITRequest>> GetAllAsync()
        {
            string sql = @"select * from HRRS_ITRequests as it 
                inner join HRRS_ITReqStatus as st on st.id=it.Status where it.IsDelete=0";

            return await _dbConnection.GetConnection().QueryAsync<HRRS_ITRequest>(sql);
        }
        public async Task<HRRS_ITRequest?> LoadRequest(int id)
        {
            string sql = @"select * from HRRS_ITRequests as it 
                inner join HRRS_ITReqStatus as st on st.id=it.Status
                where it.RequestID=@ID and it.IsDelete=0 and st.PropName!='Approved'";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<HRRS_ITRequest>(sql, new { ID = id });
        }
        public async Task<bool> UpdateAsync(HRRS_ITRequest obj)
        {
            string sql = @"
            UPDATE HRRS_ITRequests
            SET FirstName = @FirstName,
                LastName = @LastName,
                EmployeeNo = @EmployeeNo,
                Designation = @Designation,
                Department = @Department,
                Location = @Location,
                Company = @Company,
                NIC = @NIC,
                InterviewDate = @InterviewDate,
                DueStartDate = @DueStartDate,
                IsReplacement = @IsReplacement,
                Replacement = @Replacement,
                Email = @Email,
                EmailGroup = @EmailGroup,
                Computer = @Computer,
                ComputerLogin = @ComputerLogin,
                SAP = @SAP,
                WFX = @WFX,
                NewLandline = @NewLandline,
                ExistingLandline = @ExistingLandline,
                Extension = @Extension,
                SmartPhone = @SmartPhone,
                BasicPhone = @BasicPhone,
                SIM = @SIM,
                HomeAddress = @HomeAddress,
                RequestRemark = @RequestRemark,
                RequestBy = @RequestBy,
                RequestDate = @RequestDate
            WHERE RequestID = @RequestID";

            var result = await _dbConnection.GetConnection().ExecuteAsync(sql, new {
                obj.FirstName,
                obj.LastName,
                obj.EmployeeNo,
                obj.Designation,
                obj.Department,
                obj.Location,
                obj.Company,
                obj.NIC,
                obj.InterviewDate,
                obj.DueStartDate,
                obj.IsReplacement,
                obj.Replacement,
                obj.Email,
                obj.EmailGroup,
                obj.Computer,
                obj.ComputerLogin,
                obj.SAP,
                obj.WFX,
                obj.NewLandline,
                obj.ExistingLandline,
                obj.Extension,
                obj.SmartPhone,
                obj.BasicPhone,
                obj.SIM,
                obj.HomeAddress,
                obj.RequestRemark,
                RequestBy = _iSessionHelper.GetShortName().ToUpper(),
                RequestDate = DateTime.Now,
                obj.RequestID
            });
            return result > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            string sql = "UPDATE HRRS_ITRequests SET IsDelete = 1 WHERE RequestID = @ID";
            var result = await _dbConnection.GetConnection().ExecuteAsync(sql, new { ID = id });
            if (result > 0)
            {
                LogdbHRRS logdb = new()
                {
                    TrObjectId = id,
                    TrLog = "IT REQUEST DELETED"
                };
                _iHRRSLogdb.InsertLog(_dbConnection, logdb);
            }
            return result > 0;
        }

    }
}
