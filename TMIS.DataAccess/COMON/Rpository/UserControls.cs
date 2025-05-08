using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class UserControls(IDatabaseConnectionSys dbConnection) : IUserControls
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<SelectListItem>> LoadDropDownsAsync(string tableName)
        {
            string query = $@"SELECT CAST(Id AS NVARCHAR) AS Value, 
            PropName AS Text FROM {tableName} WHERE IsDelete = 0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }
    }
}
