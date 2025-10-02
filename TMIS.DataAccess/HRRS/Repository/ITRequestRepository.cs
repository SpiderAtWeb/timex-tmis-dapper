using Azure.Core;
using Dapper;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
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
         ICommonList iCommonList, IDatabaseConnectionAdm admConnection, IConfiguration configuration) : IITRequestRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IDatabaseConnectionAdm _admConnection = admConnection;
        private readonly IHRRSLogdb _iHRRSLogdb = iHRRSLogdb;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IGmailSender _gmailSender = gmailSender;
        private readonly IConfiguration _configuration = configuration;

        public async Task<Create> LoadDropDowns()
        {
            var objCreateVM = new Create();

            objCreateVM = new Create
            {
                LocationList = await _icommonList.LoadLocationsFromAD(),
                DepartmentList = await _icommonList.LoadDepartmentsFromAD(),                                
                DesignationList = await _icommonList.LoadDesignationsFromAD(),
                EmployeeList = await _icommonList.LoadEmployeeListHRRS()
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
            
                return insertedId > 0;
            }
            catch
            {

                return false;
            }
        }
        public async Task PrepairEmail(int? requestID)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"select * from HRRS_ITRequests as it 
                inner join HRRS_ITReqStatus as st on st.id=it.Status where it.RequestID=@RequestID";

            string sql = "UPDATE HRRS_ITRequests SET Status = 4 WHERE RequestID = @ID";
            var result = await connection.ExecuteAsync(sql, new { ID = requestID });

            HRRS_ITRequest? header = await connection.QueryFirstOrDefaultAsync<HRRS_ITRequest>(headerQuery, new { RequestID = requestID });
            if (header == null)
            {
                throw new InvalidOperationException("No IT request found for the given RequestID.");
            }
        
            // Send email
            await Task.Run(() => _gmailSender.ITRequestToApprove(header));
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
            try
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
                Status = @Status,
                RequestBy = @RequestBy,
                RequestDate = @RequestDate
            WHERE RequestID = @RequestID";

                var result = await _dbConnection.GetConnection().ExecuteAsync(sql, new
                {
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
                    Status = '1',
                    RequestBy = _iSessionHelper.GetShortName().ToUpper(),
                    RequestDate = DateTime.Now,
                    obj.RequestID
                });

                if(result > 0)
                {
                    LogdbHRRS logdb = new()
                    {
                        TrObjectId = obj.RequestID,
                        TrLog = "IT REQUEST UPDATED"

                    };

                    _iHRRSLogdb.InsertLog(_dbConnection, logdb);
                }

                return result > 0;
            }
            catch
            {
                return false;
            }
            
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
        public async Task<bool> ApproveAsync(HRRS_ITRequest obj, int status)
        {
            string sql = @"
            UPDATE HRRS_ITRequests
            SET Status = @Status,
                ApproverResponseDate = @ApproverResponseDate,
                ApproverRemark = @ApproverRemark              
            WHERE RequestID = @RequestID";

            var result = await _dbConnection.GetConnection().ExecuteAsync(sql, new
            {
                Status = status,                
                ApproverResponseDate = DateTime.Now,
                ApproverRemark = obj.ApproverRemark,
                obj.RequestID
            });
            if (result > 0)
            {
                if (status == 2)
                {
                    PrepairEmailForIT(obj.RequestID);
                }                
            }
            return result > 0;
        }      
        public async Task<HRRS_ITRequest?> LoadRequestForEmail(int id)
        {
            string sql = @"select * from HRRS_ITRequests as it 
                inner join HRRS_ITReqStatus as st on st.id=it.Status
                where it.RequestID=@ID and it.IsDelete=0 ";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<HRRS_ITRequest>(sql, new { ID = id });
        }
        public void PrepairEmailForIT(int? requestID)
        {
            using var connection = _dbConnection.GetConnection();

            string headerQuery = @"select * from HRRS_ITRequests as it 
                inner join HRRS_ITReqStatus as st on st.id=it.Status where it.RequestID=@RequestID";

            HRRS_ITRequest? header = connection.Query<HRRS_ITRequest>(headerQuery, new { RequestID = requestID }).FirstOrDefault();
            if (header == null)
            {
                throw new InvalidOperationException("No IT request found for the given RequestID.");
            }

            // Send email
            Task.Run(() => _gmailSender.ITRequestToIT(header));
        }
    }
}
