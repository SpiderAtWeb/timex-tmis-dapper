using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;


namespace TMIS.DataAccess.ITIS.Repository
{
    public class AttributeRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList,
                            IITISLogdb iITISLogdb) : IAttributeRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
      
        public async Task<CreateAttributeVM> LoadDropDowns()
        {
            var objCreateAttributeVM = new CreateAttributeVM(); 
           
            objCreateAttributeVM = new CreateAttributeVM
            {
                AttributeTypeList = await _icommonList.LoadAttributeTypes(),
                DeviceTypeList = await _icommonList.LoadDeviceTypes()               
            };
           
            return objCreateAttributeVM;
        }

        public async Task<IEnumerable<AttributeVM>> GetAllAsync()
        {
            string sql = @"select att.AttributeID, att.DeviceTypeID, att.Name, ty.AttributeType as DataType, att.IsRequired, dt.DeviceType as DeviceTypeName from ITIS_Attributes as att 
                            inner join ITIS_DeviceTypes as dt on dt.DeviceTypeID=att.DeviceTypeID
	                        inner join ITIS_AttributeType as ty on ty.ID=att.DataType";

            return await _dbConnection.GetConnection().QueryAsync<AttributeVM>(sql);
        }

        public async Task<bool> CheckAttributeExist(string name, string deviceTypeID)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_Attributes
            WHERE Name = @Name and DeviceTypeID=@DeviceTypeID";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { Name = name, DeviceTypeID = deviceTypeID });
            return result.HasValue;
        }

        public async Task<bool> AddAsync(AttributeModel attribute, List<AttributeListOption>? attributeListOption)
        {
            const string query = @"
            INSERT INTO ITIS_Attributes 
            (DeviceTypeID, Name, DataType)
            VALUES 
            (@DeviceTypeID, @Name, @DataType);
            SELECT CAST(SCOPE_IDENTITY() AS INT) AS InsertedId;";

            try
            {               
                var insertedId = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<int?>(query, new
                {
                    DeviceTypeID = attribute.DeviceTypeID,
                    Name = attribute.Name,
                    DataType = attribute.DataType
                });

                if (insertedId.HasValue)
                {                    
                    if (attributeListOption != null && attributeListOption.Any())
                    {
                        const string optionInsertQuery = @"INSERT INTO ITIS_AttributeListOptions 
                                            (AttributeID, OptionText) 
                                            VALUES 
                                            (@AttributeID, @OptionText);";

                        foreach (var option in attributeListOption)
                        {
                            await _dbConnection.GetConnection().ExecuteAsync(optionInsertQuery, new
                            {
                                AttributeID = insertedId.Value,
                                OptionText = option.OptionText
                            });
                        }
                    }

                    Logdb logdb = new()
                    {
                        TrObjectId = insertedId.Value,
                        TrLog = "ATTRIBUTE CREATED"

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
    }
}
