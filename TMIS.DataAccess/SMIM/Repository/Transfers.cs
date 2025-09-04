using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Transfers(IDatabaseConnectionSys dbConnection, ISMIMLogdb iSMIMLogdb, ISessionHelper sessionHelper, IGmailSender gmailSender) : ITransfers
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly IGmailSender _gmailSender = gmailSender;

        public async Task<IEnumerable<TransMC>> GetList()
        {
            string query = string.Empty;

            if (_iSessionHelper.GetUserRolesList().Contains("SUPER-ADMIN") || _iSessionHelper.GetUserRolesList().Contains("SMIM-ADMIN"))
            {
                query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 1) ORDER BY QrCode;";
            }
            else
            {
                query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 1) AND CurrentUnitId NOT IN @AccessPlants ORDER BY QrCode;";
            }

            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });

        }

        public async Task<IEnumerable<TransMCUser>> GetListUser(string pDate)
        {
            string query = "SELECT Id, QrCode, SerialNo, MachineType, CurrentUnit, TrStatusId FROM SMIM_VwMcRequest WHERE (DateTr = CONVERT(DATETIME, '" + pDate + " 00:00:00', 102))";
            return await _dbConnection.GetConnection().QueryAsync<TransMCUser>(query);
        }

        public async Task<MachinesData> GetMachineData(int pMcId)
        {
            var query = "SELECT * FROM SMIM_VwMcInventory WHERE Id = @MachineId;";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachinesData>(query, new { MachineId = pMcId })
                   ?? new MachinesData(); // Return a default instance
        }

        public async Task<IEnumerable<SelectListItem>> GetLocationsList()
        {
            var query = "SELECT Id, PropName FROM SMIM_MasterTwoSewingLines WHERE IsDelete = 0;";

            var locations = await _dbConnection.GetConnection().QueryAsync(query);
            return locations.Select(location => new SelectListItem
            {
                Value = location.Id.ToString(),
                Text = location.PropName
            });
        }

        public async Task SaveMachineTransferAsync(McRequestDetailsVM oModel)
        {
            string insertQuery = @"
            INSERT INTO [dbo].[SMIM_TrTransfers]
            ([McId], [UnitId], [LocationId], [TrStatusId], [TrUserId], [DateTr], [DateCreate], [ReqRemark], [isCompleted])
            OUTPUT INSERTED.Id
            VALUES
            (@McId, @UnitId, @LocationId, 3, @UserId, @NowDT, @NowDT, @ReqRemark, 0)";

            string updateQuery = @"
            UPDATE SMIM_TrInventory 
            SET CurrentStatusId = @NewStatus 
            WHERE Id = @MachineId";

            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var now = DateTime.Now;

                var generatedId = await connection.QuerySingleAsync<int>(insertQuery, new
                {
                    McId = oModel.oMcData!.Id,
                    UnitId = oModel.ReqUnitId,
                    LocationId = oModel.ReqLocId,
                    UserId = _iSessionHelper.GetUserId(),
                    NowDT = now,
                    oModel.ReqRemark
                }, transaction);

                await connection.ExecuteAsync(updateQuery, new
                {
                    MachineId = oModel.oMcData.Id,
                    NewStatus = 3
                }, transaction);

                await PrepairEmailAsync(connection, transaction, generatedId, oModel.oMcData.Id);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task PrepairEmailAsync(IDbConnection connection, IDbTransaction transaction, int transferId, int machineId)
        {
            string selectQuery = @"
        SELECT QrCode, SerialNo, MachineModel, MachineBrand, MachineType, 
               CurrentUnit, ReqUnit, ReqLocation, ReqRemark, Location
        FROM SMIM_VwMcRequest
        WHERE Id = @TransferId";

            var result = (await connection.QueryAsync(selectQuery, new { TransferId = transferId }, transaction)).FirstOrDefault();

            if (result == null)
                throw new Exception($"No data found for TransferId: {transferId}");

            string[] details =
            [
                transferId.ToString(),
                result.QrCode,
                result.SerialNo,
                result.MachineModel,
                result.MachineBrand,
                result.MachineType,
                result.CurrentUnit,
                result.Location,
                result.ReqUnit,
                result.ReqLocation,
                result.ReqRemark
            ];

            var logEntry = new Logdb
            {
                TrObjectId = machineId,
                TrLog = $"MACHINE REQUESTED FROM [{result.ReqUnit}] - TO [{result.ReqLocation}]"
            };

            _iSMIMLogdb.InsertLog(connection, logEntry, transaction);

            // Send email outside of transaction to avoid external dependency in transaction scope
            _gmailSender.McRequestToApprove(details);
        }


        public IEnumerable<TransMC> GetRequestList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] = 3;";
            return _dbConnection.GetConnection().Query<TransMC>(query);
        }
    }
}
