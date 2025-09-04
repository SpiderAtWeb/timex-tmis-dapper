using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class SMIMCommon(IDatabaseConnectionSys dbConnection, ISessionHelper sessionHelper) : ISMIMCommon
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<SelectListItem>> GetUnitsList()
        {
            var query = "SELECT Id, PropName FROM COMN_VwTwoCompLocs WHERE IsDelete = 0 AND Id IN @AccessPlants ORDER BY PropName";

            var units = await _dbConnection.GetConnection().QueryAsync(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
            return units.Select(unit => new SelectListItem
            {
                Value = unit.Id.ToString(),
                Text = unit.PropName
            });
        }
    }
}
