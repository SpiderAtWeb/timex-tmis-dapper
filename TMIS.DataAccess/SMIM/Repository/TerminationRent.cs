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
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 0) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
        }

        public async Task<MachineRentedVM?> GetRentedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DateBorrow, DateDue, ServiceSeq, MachineBrand, MachineType, Location, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, Supplier, CostMethod, Comments,
            LastScanDateTime FROM SMIM_VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineRentedVM>(query, new { Id = id });
        }

        public async Task SaveMachineObsoleteAsync(MachineRentedVM oModel)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            var queryUpdate = @"UPDATE SMIM_TrInventory SET [CurrentStatusId] = @NewStatus,
                                [RentTermDate]= @RentTermDate,
                                [RentTermRemark]= @RentTermRemark,
                                [DateUpdate] = @Now WHERE Id = @MachineId";

            try
            {
                //Update TrMachineInventory status
                await connection.ExecuteAsync(queryUpdate, new
                {
                    MachineId = oModel.Id,
                    NewStatus = 10,
                    oModel.RentTermRemark,
                    RentTermDate = oModel.RentTerminationDate,
                    DateTime.Now,
                }, transaction);

                Logdb logdb = new()
                {
                    TrObjectId = oModel.Id,
                    TrLog = "MACHINE RENT AGREEMENT TERMINATED"
                };

                _iSMIMLogdb.InsertLog(connection, logdb, transaction);

                // Commit the transaction
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
