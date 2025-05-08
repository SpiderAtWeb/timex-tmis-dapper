using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class TerminationRent(IDatabaseConnectionSys dbConnection, ISMIMLogdb iSMIMLogdb, ISessionHelper sessionHelper) : ITerminationRent
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<TransMC>> GetList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 0) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetAccessPlantsArray() });
        }

        public async Task<MachineRentedVM?> GetRentedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DateBorrow, DateDue, ServiceSeq, MachineBrand, MachineType, CompanyGroup, Location, OwnedCluster, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, Supplier, CostMethod, Comments,
            LastScanDateTime FROM VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineRentedVM>(query, new { Id = id });
        }

        public async Task SaveMachineObsoleteAsync(MachineRentedVM oModel)
        {
            var queryUpdate = @"UPDATE SMIM_TrMachineInventory SET [CurrentStatusId] = @NewStatus, [RentTermRemark]= @RentTermRemark, [DateUpdate] = @Now WHERE Id = @MachineId";

            _dbConnection.GetConnection().Open();
            using (var trns = _dbConnection.GetConnection().BeginTransaction())
            {
                try
                {
                    //Update TrMachineInventory status
                    await _dbConnection.GetConnection().ExecuteAsync(queryUpdate, new
                    {
                        MachineId = oModel.Id,
                        NewStatus = 10,
                        oModel.RentTermRemark,
                        DateTime.Now,
                    }, transaction: trns);


                    // Commit the transaction
                    trns.Commit();

                    Logdb logdb = new()
                    {
                        TrObjectId = oModel.Id,
                        TrLog = "MACHINE RENT AGREEMENT TERMINATED"
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
