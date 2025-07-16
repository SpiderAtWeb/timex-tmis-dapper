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
                EmployeeList = await _icommonList.LoadEmployeeList()
            };
            return objCreateVM;
        }
        public async Task<bool> AddAsync(Create obj)
        {
            const string query = @"
            INSERT INTO HRRS_ITRequests
            (FirstName, LastName, EmployeeNo, Designation, Department, Location, Company, NIC, InterviewDate, DueStartDate,
            RecruitmentType, Replacement, Computer, Email, EmailGroup, ComputerLogin, SAPAccess, WFXAccess, Landnewline,
            ExistingLandLine, Extension, SmartPhone, BasicPhone, SIM, HomeAddress, RequestRemark, Status)
            VALUES
            (@FirstName, @LastName, @EmployeeNo, @Designation, @Department, @Location, @Company, @NIC, @InterviewDate, @DueStartDate,
            @RecruitmentType, @Replacement, @Computer, @Email, @EmailGroup, @ComputerLogin, @SAPAccess, @WFXAccess, @Landnewline,
            @ExistingLandLine, @Extension, @SmartPhone, @BasicPhone, @SIM, @HomeAddress, @RequestRemark, @Status);
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
                    obj.HRRS_ITRequest!.RecruitmentType,
                    obj.HRRS_ITRequest!.Replacement,
                    obj.HRRS_ITRequest!.Computer,
                    obj.HRRS_ITRequest!.Email,
                    obj.HRRS_ITRequest!.EmailGroup,
                    obj.HRRS_ITRequest!.ComputerLogin,
                    obj.HRRS_ITRequest!.SAPAccess,
                    obj.HRRS_ITRequest!.WFXAccess,
                    obj.HRRS_ITRequest!.Landnewline,
                    obj.HRRS_ITRequest!.ExistingLandLine,
                    obj.HRRS_ITRequest!.Extension,
                    obj.HRRS_ITRequest!.SmartPhone,
                    obj.HRRS_ITRequest!.BasicPhone,
                    obj.HRRS_ITRequest!.SIM,
                    obj.HRRS_ITRequest!.HomeAddress,              
                    obj.HRRS_ITRequest!.RequestRemark,
                    Status = 1 // Default to 'Pending' or adjust as needed
                });
                
                int? id = insertedId;

                PrepairEmail(id);

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
