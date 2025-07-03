using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;


namespace TMIS.DataAccess.SMIM.Repository
{
    public class Disposal(IDatabaseConnectionSys dbConnection, ISMIMLogdb iSMIMLogdb, ISessionHelper sessionHelper) : IDisposal
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<TransMC>> GetList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 1) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
        }

        public async Task<MachinesData> GetMachineData(int pMcId)
        {
            var query = "SELECT * FROM SMIM_VwMcInventory WHERE Id = @MachineId;";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachinesData>(query, new { MachineId = pMcId })
                   ?? new MachinesData(); // Return a default instance
        }

        public async Task SaveMachineObsoleteAsync(MachinesData oModel)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            var queryUpdate = @"UPDATE SMIM_TrInventory SET [CurrentStatusId] = @NewStatus, [Comments]= @Comments, [DateUpdate] = @Now WHERE Id = @MachineId";

            try
            {
                //Update TrMachineInventory status
                await _dbConnection.GetConnection().ExecuteAsync(queryUpdate, new
                {
                    MachineId = oModel.Id,
                    NewStatus = 9,
                    oModel.Comments,
                    DateTime.Now,
                }, transaction);


                Logdb logdb = new()
                {
                    TrObjectId = oModel.Id,
                    TrLog = "MACHINE OBSOLETED"
                };

                _iSMIMLogdb.InsertLog(connection, logdb, transaction);

                transaction.Commit();
            }
            catch
            {
                // Rollback the transaction if any command fails
                transaction.Rollback();
                throw;
            }

        }
    }
}
