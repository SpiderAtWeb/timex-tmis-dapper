using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.HRRS.IRepository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.Models.HRRS.VM;
using TMIS.Models.ITIS.VM;
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
            };
            return objCreateVM;
        }
        public async Task<bool> AddAsync(Create obj)
        {
            const string query = @"
                INSERT INTO HRRS_ITRequests
                (EmployeeName, EmployeeNo, Designation, Department, Location, Company, NIC, InterviewDate, DueStartDate,
                RecruitmentType, Computer, ComputerGroup, ComputerLogin, SAPAccess, Landnewline, ExistingLandLine, Extension,
                SmartPhone, BasicPhone, SMSOnly, RequestDate, RequestRemark, Status)
                VALUES
                (@EmployeeName, @EmployeeNo, @Designation, @Department, @Location, @Company, @NIC, @InterviewDate, @DueStartDate,
                @RecruitmentType, @Computer, @ComputerGroup, @ComputerLogin, @SAPAccess, @Landnewline, @ExistingLandLine, @Extension,
                @SmartPhone, @BasicPhone, @SMSOnly, @RequestDate, @RequestRemark, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    EmployeeName = obj.EmployeeName,
                    EmployeeNo = obj.EmployeeNo,
                    Designation = obj.Designation,
                    Department = obj.Department,
                    Location = obj.Location,
                    Company = obj.Company,
                    NIC = obj.NIC,
                    InterviewDate = obj.InterviewDate,
                    DueStartDate = obj.DueStartDate,
                    RecruitmentType = obj.RecruitmentType,
                    Computer = obj.Computer,
                    ComputerGroup = obj.ComputerGroup,
                    ComputerLogin = obj.ComputerLogin,
                    SAPAccess = obj.SAPAccess,
                    Landnewline = obj.Landnewline,
                    ExistingLandLine = obj.ExistingLandLine,
                    Extension = obj.Extension,
                    SmartPhone = obj.SmartPhone,
                    BasicPhone = obj.BasicPhone,
                    SMSOnly = obj.SMSOnly,                    
                    RequestRemark = obj.RequestRemark,
                    Status = 1 // 1 is the status for 'Pending'
                });
                
                int? id = insertedId;

                //PrepairEmail(id);

                return insertedId > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
        private void PrepairEmail(int? genId)
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

    }
}
