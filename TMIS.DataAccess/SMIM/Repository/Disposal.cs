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
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 1) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetAccessPlantsArray() });
        }

        public async Task<MachinesData> GetMachineData(int pMcId)
        {
            var query = "SELECT * FROM VwMcInventory WHERE Id = @MachineId;";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachinesData>(query, new { MachineId = pMcId })
                   ?? new MachinesData(); // Return a default instance
        }

        public async Task SaveMachineObsoleteAsync(MachinesData oModel)
        {
            var queryUpdate = @"UPDATE SMIM_TrMachineInventory SET [CurrentStatusId] = @NewStatus, [Comments]= @Comments, [DateUpdate] = @Now WHERE Id = @MachineId";

            _dbConnection.GetConnection().Open();
            using (var trns = _dbConnection.GetConnection().BeginTransaction())
            {
                try
                {
                    //Update TrMachineInventory status
                    await _dbConnection.GetConnection().ExecuteAsync(queryUpdate, new
                    {
                        MachineId = oModel.Id,
                        NewStatus = 9,
                        oModel.Comments,
                        DateTime.Now,
                    }, transaction: trns);


                    // Commit the transaction
                    trns.Commit();

                    Logdb logdb = new()
                    {
                        TrObjectId = oModel.Id,
                        TrLog = "MACHINE OBSOLETED"
                    };

                    _iSMIMLogdb.InsertLog(_dbConnection, logdb);
                }
                catch
                {
                    // Rollback the transaction if any command fails
                    trns.Rollback();
                    throw;
                }
            }
        }
    }
}
