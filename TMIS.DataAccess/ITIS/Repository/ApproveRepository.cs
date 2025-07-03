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
    public class ApproveRepository(IDatabaseConnectionSys dbConnection,
                            ISessionHelper sessionHelper,
                            IITISLogdb iITISLogdb) : IApproveRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<ApproveDeviceUserVM>> GetAllAsync()
        {
            // GetUserId
            string sql = @"select a.AssignmentID, a.DeviceID, t.DeviceType, d.DeviceName, d.SerialNumber, ISNull(ad.EmpName, a.EmpName) as EmpName, a.Designation, 
                            a.AssignedDate, a.AssignLocation as LocationName, a.AssignDepartment as DepartmentName, st.PropName as AssignStatus 
                            from ITIS_DeviceAssignments as a 
                            left join ITIS_MasterADEmployees as ad on ad.EmpUserName=a.EmpName
                            left join ITIS_Devices as d on d.DeviceID=a.DeviceID
                            left join ITIS_DeviceTypes as t on t.DeviceTypeID=d.DeviceTypeID                  
                            left join ITIS_DeviceAssignStatus as st on st.Id=a.AssignStatusID                           
                            where a.AssignStatusID = 2 and a.ApproverEMPNo = @ApproverEMPNo"; 

            return await _dbConnection.GetConnection().QueryAsync<ApproveDeviceUserVM>(sql, new {
                ApproverEMPNo = _iSessionHelper.GetUserId()
            });
        }

        public async Task<ApproveVM?> GetSelectedRecord(int assignmentID)
        {
            string sql = @"select da.AssignmentID, da.DeviceID,  ISNull(ad.EmpName, da.EmpName) as EmpName, 
							da.AssignLocation, da.AssignDepartment, da.Designation, da.AssignedDate, da.AssignRemarks, d.DeviceName, 
                            d.SerialNumber, d.FixedAssetCode, d.Image1Data, d.Image2Data,
                            d.Image3Data, d.Image4Data, d.PurchasedDate, d.depreciation, d.IsRented, 
                            d.IsBrandNew from ITIS_DeviceAssignments as da 
                            inner join ITIS_Devices as d on d.DeviceID=da.DeviceID
							left join ITIS_MasterADEmployees as ad on ad.EmpUserName=da.EmpName
                            where da.AssignmentID=@AssignmentID";

            var deviceDetails =  await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<ApproveVM>(sql, new {

                AssignmentID=assignmentID
            });

            string attributeQuery = @"select a.Name, av.ValueText from ITIS_DeviceAttributeValues as av 
                                     inner join ITIS_Attributes as a on a.AttributeID=av.AttributeID where av.DeviceID=@DeviceID";

            var attributeValue = await _dbConnection.GetConnection().QueryAsync<AttributeValue>(attributeQuery, new
            {
                DeviceID = deviceDetails!.DeviceID
            });

            ApproveVM approveVM = new ApproveVM()
            {
                AssignmentID = assignmentID,
                DeviceID = deviceDetails.DeviceID,
                EmpName = deviceDetails.EmpName,
                AssignedDate = deviceDetails.AssignedDate,
                AssignRemarks = deviceDetails.AssignRemarks,
                DeviceName = deviceDetails.DeviceName,
                SerialNumber = deviceDetails.SerialNumber,
                FixedAssetCode = deviceDetails.FixedAssetCode,
                Image1Data = deviceDetails.Image1Data,
                Image2Data = deviceDetails.Image2Data,
                Image3Data = deviceDetails.Image3Data,
                Image4Data = deviceDetails.Image4Data,
                PurchasedDate = deviceDetails.PurchasedDate,
                Depreciation   = deviceDetails.Depreciation,
                IsBrandNew = deviceDetails.IsBrandNew,
                IsRented = deviceDetails.IsRented,
                AttributeValues = attributeValue.ToList(),
                AssignDepartment = deviceDetails.AssignDepartment,
                AssignLocation = deviceDetails.AssignLocation,
                Designation = deviceDetails.Designation
            };

            return approveVM;
        }

        public async Task<bool> AddAsync(ApproveVM approveVM)
        {
            try
            {
                string query = @"update ITIS_DeviceAssignments  set AssignStatusID=3, ApproverResponseDate=getDate(), ApproverRemark=@ApproveRemark
                            where AssignmentID=@AssignmentID";

                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    AssignmentID = approveVM.AssignmentID,
                    ApproveRemark = approveVM.ApproverRemark
                });

                if (rowsAffected > 0)
                {
                    Logdb logdb = new()
                    {
                        TrObjectId = approveVM.AssignmentID,
                        TrLog = "DEVICE ASSIGNMENT APPROVED"

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

        public async Task<bool> Reject(ApproveVM approveVM)
        {
            try
            {
                string query = @"update ITIS_DeviceAssignments  set AssignStatusID=4, ApproverResponseDate=getDate(), ApproverRemark=@ApproveRemark
                            where AssignmentID=@AssignmentID";

                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(query, new
                {
                    AssignmentID = approveVM.AssignmentID,
                    ApproveRemark = approveVM.ApproverRemark
                });

                if (rowsAffected > 0)
                {
                    const string deviceQuery = @"update ITIS_DEVICES set DeviceStatusID=6 where DeviceID=@DeviceID";

                    _dbConnection.GetConnection().Execute(deviceQuery, new
                    {
                        DeviceID = approveVM.DeviceID
                    });

                    Logdb logdb = new()
                    {
                        TrObjectId = approveVM.AssignmentID,
                        TrLog = "DEVICE ASSIGNMENT REJECT"

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

    }
}
