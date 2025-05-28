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
            string sql = @"select da.AssignmentID, da.EmpName, da.ApproverEMPNo, CAST(da.AssignedDate AS DATE) AS AssignedDate,
                            d.SerialNumber from ITIS_DeviceAssignments as da 
                            inner join ITIS_Devices as d on d.DeviceID = da.DeviceID
                            where da.AssignStatusID=2 and da.ApproverEMPNo=1"; // replace with actual approver

            return await _dbConnection.GetConnection().QueryAsync<ApproveDeviceUserVM>(sql);
        }

        public async Task<ApproveVM?> GetSelectedRecord(int assignmentID)
        {
            string sql = @"select da.AssignmentID, da.DeviceID, da.EmpName, da.AssignedDate, da.AssignRemarks, d.DeviceName, 
                            d.SerialNumber, d.FixedAssetCode, d.Image1Data, d.Image2Data,
                            d.Image3Data, d.Image4Data, d.PurchasedDate, d.depreciation, d.IsRented, 
                            d.IsBrandNew from ITIS_DeviceAssignments as da 
                            inner join ITIS_Devices as d on d.DeviceID=da.DeviceID
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
                AttributeValues = attributeValue.ToList()
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
