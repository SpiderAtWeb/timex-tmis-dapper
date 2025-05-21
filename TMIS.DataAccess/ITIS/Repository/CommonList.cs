using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class CommonList(IDatabaseConnectionSys dbConnection) : ICommonList
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        public async Task<IEnumerable<SelectListItem>> LoadAttributeTypes()
        {
            string query = @"SELECT ID AS Value, 
            AttributeType AS Text FROM ITIS_AttributeType ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadDeviceTypes()
        {
            string query = @"SELECT DeviceTypeID AS Value, 
            DeviceType AS Text FROM ITIS_DeviceTypes WHERE IsDelete = 0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<IEnumerable<SelectListItem>> LoadLocations()
        {
            string query = @"select id as Value, LocationName AS Text from COMN_MasterTwoLocations 
                            where IsDelete=0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        
        public async Task<IEnumerable<SelectListItem>> LoadDeviceStatus()
        {
            string query = @"select Id as Value, PropName AS Text from ITIS_DeviceStatus 
                            where IsDelete=0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadVendors()
        {
            string query = @"select ID as Value, Name AS Text from ITIS_VendorTemp 
                            where IsDelete=0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<IEnumerable<SelectListItem>> LoadInstoreSerialList()
        {
            string query = @"select DeviceID as Value , SerialNumber as Text from ITIS_Devices where DeviceStatusID=6";
            //only get instore devices
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results; 
        }
        public async Task<IEnumerable<SelectListItem>> LoadEmployeeList()
        {
            string query = @"select EmpNo as Value, CAST(EmpNo AS VARCHAR) + ' - ' + EmpName AS Text from ITIS_EmployeeTemp where EmpAct='A'";
            //replace with real datasource
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<DeviceDetailVM> LoadDeviceDetail(int deviceID)
        {
            try
            {
                string query = @"select DeviceName, SerialNumber, FixedAssetCode, PurchasedDate,depreciation, IsRented, IsBrandNew 
                             from ITIS_Devices where DeviceID=@DeviceID";

                DeviceDetailVM deviceDetailVM = new DeviceDetailVM();
                var deviceDetail = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceDetailVM>(query, new
                {
                    DeviceID = deviceID
                });

                deviceDetailVM = deviceDetail;

                string attributeQuery = @"select a.Name, av.ValueText from ITIS_DeviceAttributeValues as av 
                                     inner join ITIS_Attributes as a on a.AttributeID=av.AttributeID where av.DeviceID=@DeviceID";

                var attributeValue = await _dbConnection.GetConnection().QueryAsync<AttributeValue>(attributeQuery, new
                {
                    DeviceID = deviceID
                });

                deviceDetailVM.AttributeValues = attributeValue.ToList();

                return deviceDetailVM;
            }
            catch(Exception ex)  
            {
                return null;
            }

        }

        public async Task<DeviceUserDetailVM> LoadUserDetail(int deviceID)
        {
            try
            {
                string query = @"select AssignmentID, EMPNo, EMPName, Designation, AssignedDate, AssignRemarks
                            , ApproverEMPNo, ApproverResponseDate, ApproverRemark 
                            from ITIS_DeviceAssignments where DeviceID=@DeviceID and AssignStatusID not in (4, 5)";

                DeviceUserDetailVM? deviceUserDetail = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceUserDetailVM>(query, new
                {
                    DeviceID = deviceID
                });

                return deviceUserDetail;
            }
            catch (Exception ex) 
            {
                return null;
            }

           
        }

        public async Task<IEnumerable<SelectListItem>> LoadInUseSerialList()
        {
            string query = @"select DeviceID as Value , SerialNumber as Text from ITIS_Devices where DeviceStatusID=7";
            //only get inuse devices
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

    }
}
