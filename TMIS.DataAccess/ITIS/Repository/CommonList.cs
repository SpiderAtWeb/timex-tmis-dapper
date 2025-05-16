using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
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
       
    }
}
