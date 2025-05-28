using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
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
      
        public async Task<CreateAttributeVM> LoadDropDowns(int? id)
        {
            var objCreateAttributeVM = new CreateAttributeVM();

            objCreateAttributeVM = new CreateAttributeVM
            {
                AttributeTypeList = await _icommonList.LoadAttributeTypes(),
                DeviceTypeList = await _icommonList.LoadDeviceTypes()
            };

            if (id != null)
            {
                objCreateAttributeVM.Attribute = await LoadAttributeDetails(id);

                if (objCreateAttributeVM.Attribute != null && objCreateAttributeVM.Attribute.DataType == "1")
                {
                    objCreateAttributeVM.AttributeListOption = (await LoadAttributeListOptions(objCreateAttributeVM.Attribute.AttributeID)).ToList();
                }
            }
                           
            return objCreateAttributeVM;
        }

        public async Task<IEnumerable<AttributeListOption>> LoadAttributeListOptions(int id)
        {
            string sql = @"select * from ITIS_AttributeListOptions where AttributeID=@AttributeID";

            return await _dbConnection.GetConnection().QueryAsync<AttributeListOption>(sql, new
            {
                AttributeID= id
            });
        }

        public async Task<AttributeModel?> LoadAttributeDetails(int? id)
        {
            string sql = @"select AttributeID, DeviceTypeID, Name, DataType from ITIS_Attributes Where AttributeID=@AttributeID and IsDelete=0";

            return await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<AttributeModel>(sql, new
            {
                AttributeID=id
            });
        }

        public async Task<IEnumerable<AttributeVM>> GetAllAsync()
        {
            string sql = @"select att.AttributeID, att.DeviceTypeID, att.Name, ty.AttributeType as DataType, att.IsRequired, dt.DeviceType as DeviceTypeName from ITIS_Attributes as att 
                            inner join ITIS_DeviceTypes as dt on dt.DeviceTypeID=att.DeviceTypeID
	                        inner join ITIS_AttributeType as ty on ty.ID=att.DataType
                            where dt.IsDelete=0 and att.IsDelete=0";

            return await _dbConnection.GetConnection().QueryAsync<AttributeVM>(sql);
        }

        public async Task<bool> CheckAttributeExist(string name, string deviceTypeID)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_Attributes
            WHERE Name = @Name and DeviceTypeID=@DeviceTypeID and IsDelete=0";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new { Name = name, DeviceTypeID = deviceTypeID });
            return result.HasValue;
        }

        public async Task<bool> CheckAttributeExist(CreateAttributeVM obj)
        {
            const string query = @"
            SELECT TOP 1 1
            FROM ITIS_Attributes
            WHERE Name = @Name and DeviceTypeID=@DeviceTypeID and AttributeID!=@AttributeID and IsDelete=0";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<int?>(query, new {
                Name = obj.Attribute!.Name,
                DeviceTypeID = obj.Attribute.DeviceTypeID,
                AttributeID= obj.Attribute!.AttributeID });
            return result.HasValue;
        }

        public async Task<bool> UpdateAttribute(AttributeModel attribute, List<AttributeListOption>? attributeListOption)
        {         
            using (var trns = _dbConnection.GetConnection().BeginTransaction())
            {
                try
                {
                    const string attributeQuery = @"Update ITIS_Attributes SET
                                                DeviceTypeID=@DeviceTypeID, Name=@Name, DataType=@DataType where AttributeID=@AttributeID;";

                    int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(attributeQuery, new
                    {
                        AttributeID = attribute.AttributeID,
                        DeviceTypeID = attribute.DeviceTypeID,
                        DataType = attribute.DataType,
                        Name = attribute.Name
                    }, trns);

                    if (rowsAffected > 0)
                    {
                        const string deleteAttributeOption = @"DELETE FROM ITIS_AttributeListOptions where AttributeID=@AttributeID;";

                        await _dbConnection.GetConnection().ExecuteAsync(deleteAttributeOption, new
                        {
                            AttributeID = attribute.AttributeID                           
                        }, trns);

                        if (attribute.DataType=="1" && attributeListOption != null && attributeListOption.Any())
                        {
                            const string attributeOption = @"INSERT INTO ITIS_AttributeListOptions
                                (AttributeID,OptionText)
                                VALUES (@AttributeID,@OptionText)";

                            foreach (var option in attributeListOption)
                            {
                                await _dbConnection.GetConnection().ExecuteAsync(attributeOption, new
                                {
                                    AttributeID = attribute.AttributeID,
                                    OptionText = option.OptionText
                                }, trns);
                            }
                        }

                        // Commit the transaction
                        trns.Commit();                                               
                    }

                    Models.ITIS.Logdb logdb = new()
                    {
                        TrObjectId = attribute.AttributeID,
                        TrLog = "ATTRIBUTE UPDATED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);

                    return true;
                }
                catch
                {
                    // Rollback the transaction if any command fails
                    trns.Rollback();
                    return false;
                }
            }
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

        public async Task<bool> DeleteAttribute(AttributeModel attribute)
        {
            using (var trns = _dbConnection.GetConnection().BeginTransaction())
            {
                try
                {
                    const string attributeQuery = @"Update ITIS_Attributes SET
                                                IsDelete=1 where AttributeID=@AttributeID;";

                    int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(attributeQuery, new
                    {
                        AttributeID = attribute.AttributeID
                    }, trns);

                    if (rowsAffected > 0)
                    {                                               
                        // Commit the transaction
                        trns.Commit();
                    }

                    Models.ITIS.Logdb logdb = new()
                    {
                        TrObjectId = attribute.AttributeID,
                        TrLog = "ATTRIBUTE DELETED"

                    };

                    _iITISLogdb.InsertLog(_dbConnection, logdb);

                    return true;
                }
                catch
                {
                    // Rollback the transaction if any command fails
                    trns.Rollback();
                    return false;
                }
            }
        }
    }
}
