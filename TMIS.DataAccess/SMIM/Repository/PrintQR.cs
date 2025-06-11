using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class PrintQR(IDatabaseConnectionSys dbConnection) : IPrintQR
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<string>> GetQrCode()
        {
            string query = "SELECT QrCode FROM SMIM_TrInventory WHERE (IsDelete = 0) ORDER BY QrCode";
            return await _dbConnection.GetConnection().QueryAsync<string>(query);

        }

    }
}
