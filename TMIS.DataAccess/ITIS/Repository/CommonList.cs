using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class CommonList(IDatabaseConnectionSys dbConnection, IDatabaseConnectionAdm dbConnectionAdm) : ICommonList
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IDatabaseConnectionAdm _dbConnectionAdm = dbConnectionAdm;
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
            string query = @"select id as Value, PropName AS Text from COMN_MasterTwoLocations 
                            where IsDelete=0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<IEnumerable<SelectListItem>> LoadDepartments()
        {
            string query = @"select DepartmentID as Value, DepartmentName AS Text from COMN_MasterDepartments 
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
            string query = @"select EmpUserName as Value, EmpUserName + ' - ' + EmpEmail AS Text from ITIS_MasterADEMPLOYEES where IsDelete=0";
            //replace with real datasource
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<IEnumerable<SelectListItem>> LoadApproverList()
        {
            string query = @"SELECT * FROM ITIS_VwApprovers";
            //replace with real datasource
            var results = await _dbConnectionAdm.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<DeviceDetailVM> LoadDeviceDetail(int? deviceID)
        {

            string query = @"select d.DeviceName, d.SerialNumber, d.FixedAssetCode, d.PurchasedDate,d.depreciation, d.Remark,
                            d.IsRented, d.IsBrandNew, v.Name as Vendor, d.Image1Data, d.Image2Data, d.Image3Data, d.Image4Data,
                            s.PropName as Status, l.PropName as Location
                            from ITIS_Devices as d
                            left join ITIS_VendorTemp as v on v.ID=d.VendorID
                            left join ITIS_DeviceStatus as s on s.Id = d.DeviceStatusID
                            left join COMN_MasterTwoLocations as l on l.Id = d.Location  where d.DeviceID=@DeviceID";

            DeviceDetailVM deviceDetailVM = new DeviceDetailVM();
            var deviceDetail = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceDetailVM>(query, new
            {
                DeviceID = deviceID
            });

            deviceDetailVM = deviceDetail!;

            string attributeQuery = @"select a.Name, av.ValueText from ITIS_DeviceAttributeValues as av 
                                     inner join ITIS_Attributes as a on a.AttributeID=av.AttributeID where av.DeviceID=@DeviceID";

            var attributeValue = await _dbConnection.GetConnection().QueryAsync<AttributeValue>(attributeQuery, new
            {
                DeviceID = deviceID
            });

            deviceDetailVM.AttributeValues = attributeValue.ToList();

            return deviceDetailVM;


        }

        public async Task<DeviceUserDetailVM> LoadUserDetail(int? deviceID)
        {
            string query = @"select a.AssignmentID, a.EMPNo, cm.EmpName as EMPName, a.Designation, a.AssignedDate, a.AssignRemarks, st.PropName as AssignStatus
                            , a.ApproverEMPNo, a.ApproverResponseDate, a.ApproverRemark, a.AssignDepartment as AssignDepartment, a.AssignLocation as AssignLocation
                            from ITIS_DeviceAssignments as a 
                            inner join ITIS_DeviceAssignStatus as st on st.Id=a.AssignStatusID
                            left join ITIS_MasterADEMPLOYEES as cm on cm.EmpUserName=a.EmpName
                            where a.DeviceID=@DeviceID and a.AssignStatusID not in (4, 5)";

            DeviceUserDetailVM? deviceUserDetail = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<DeviceUserDetailVM>(query, new
            {
                DeviceID = deviceID
            });

            return deviceUserDetail!;


        }

        public async Task<IEnumerable<SelectListItem>> LoadInUseSerialList()
        {
            string query = @"select DeviceID as Value , SerialNumber as Text from ITIS_Devices where DeviceStatusID=7";
            //only get inuse devices
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<IEnumerable<SelectListItem>> LoadLocationsFromAD()
        {
            const string sql = @"SELECT DISTINCT EmpLocation as Value, EmpLocation as Text FROM ITIS_MasterADEMPLOYEES WHERE EmpLocation IS NOT NULL
                                ORDER BY EmpLocation;";

            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(sql);

            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadDepartmentsFromAD()
        {
            const string sql = @"SELECT DISTINCT EmpDepartment as Value, EmpDepartment as Text FROM ITIS_MasterADEMPLOYEES WHERE EmpDepartment IS NOT NULL
                                 ORDER BY EmpDepartment;";

            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(sql);

            return results;
        }
        public async Task<IEnumerable<SelectListItem>> LoadDesignationsFromAD()
        {
            const string sql = @"SELECT DISTINCT EmpDesignation as Value, EmpDesignation as Text FROM ITIS_MasterADEMPLOYEES WHERE EmpDesignation IS NOT NULL
                                ORDER BY EmpDesignation;";

            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(sql);

            return results;
        }
    }
}
