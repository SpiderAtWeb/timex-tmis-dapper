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

            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 1) AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
        }

        public async Task<IEnumerable<TransMCUser>> GetListUser(string pDate)
        {
            string query = "SELECT Id, QrCode, SerialNo, MachineType, CurrentUnit, TrStatusId FROM VwMcRequest WHERE (DateTr = CONVERT(DATETIME, '" + pDate + " 00:00:00', 102))";
            return await _dbConnection.GetConnection().QueryAsync<TransMCUser>(query);
        }

        public async Task<MachinesData> GetMachineData(int pMcId)
        {
            var query = "SELECT * FROM VwMcInventory WHERE Id = @MachineId;";
            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachinesData>(query, new { MachineId = pMcId })
                   ?? new MachinesData(); // Return a default instance
        }

        public async Task<IEnumerable<SelectListItem>> GetLocationsList()
        {
            var query = "SELECT Id, PropName FROM SMIM_MdCompanyLocations WHERE IsDelete = 0;";


            var locations = await _dbConnection.GetConnection().QueryAsync(query);
            return locations.Select(location => new SelectListItem
            {
                Value = location.Id.ToString(),
                Text = location.PropName
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetUnitsList()
        {
            var query = "SELECT Id, PropName FROM SMIM_MdCompanyUnits WHERE IsDelete = 0 AND Id NOT IN @AccessPlants ORDER BY PropName";

            var units = await _dbConnection.GetConnection().QueryAsync(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
            return units.Select(unit => new SelectListItem
            {
                Value = unit.Id.ToString(),
                Text = unit.PropName
            });
        }

        public async Task SaveMachineTransferAsync(McRequestDetailsVM oModel)
        {
            string queryInsert = @"
            INSERT INTO [dbo].[SMIM_TrMachineTransfers]
            ([McId], [UnitId], [LocationId], [TrStatusId], [TrUserId], [DateTr], [DateCreate], [ReqRemark], [isCompleted])
            OUTPUT INSERTED.Id
            VALUES
            (@McId, @UnitId, @LocationId, 3, @UserId, @NowDT, @NowDT, @ReqRemark, 0)";

            var queryUpdate = @"UPDATE SMIM_TrMachineInventory SET CurrentStatusId = @NewStatus WHERE Id = @MachineId";

            _dbConnection.GetConnection().Open();
            using (var transaction = _dbConnection.GetConnection().BeginTransaction())
            {
                try
                {
                    // Insert into TrMachineTransfers and get the generated Id
                    var generatedId = await _dbConnection.GetConnection().QuerySingleAsync<int>(queryInsert, new
                    {
                        McId = oModel.oMcData!.Id,
                        UnitId = oModel.ReqUnitId,
                        LocationId = oModel.ReqLocId,
                        UserId = _iSessionHelper.GetUserId(),
                        NowDT = DateTime.Now,
                        oModel.ReqRemark
                    }, transaction: transaction);

                    // Update TrMachineInventory status
                    await _dbConnection.GetConnection().ExecuteAsync(queryUpdate, new
                    {
                        MachineId = oModel.oMcData.Id,
                        NewStatus = 3
                    }, transaction: transaction);

                    // Commit the transaction
                    transaction.Commit();

                    PrepairEmail(generatedId, oModel.oMcData!.Id);
                }
                catch
                {
                    // Rollback the transaction if any command fails
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void PrepairEmail(int genId, int mcId)
        {
            string query = @"
            SELECT QrCode, SerialNo, MachineModel, MachineBrand, MachineType, 
                           CurrentUnit, Location, ReqUnit, ReqLocation, ReqRemark
            FROM VwMcRequest
            WHERE (Id = @GenId)";

            // Execute the query and fetch the results
            var results = _dbConnection.GetConnection().Query(query, new { GenId = genId })
                .FirstOrDefault();

            string[] myArray = new string[]
            {
                genId.ToString(),               // Assuming genId is the entry number
                results!.QrCode,     // QR Code
                results!.SerialNo,   // Serial Number
                results!.MachineModel, // Machine Model
                results!.MachineBrand, // Machine Brand
                results!.MachineType,  // Machine Type
                results!.CurrentUnit,  // Current Unit
                results!.Location,     // Location
                results!.ReqUnit,      // Requested Unit
                results!.ReqLocation,  // Requested Location
                results!.ReqRemark     // Requested Remark
            };

            Logdb logdb = new()
            {
                TrObjectId = mcId,
                TrLog = $"MACHINE REQUESTED FROM [{results!.ReqUnit}] - TO [{results!.ReqLocation}]"
            };

            _iSMIMLogdb.InsertLog(_dbConnection, logdb);

            // Send results to Gmail sender
            _gmailSender.McRequestToApprove(myArray);

        }

        public IEnumerable<TransMC> GetRequestList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [CurrentStatus], [Location] FROM [VwMcInventory] WHERE [CurrentStatus] = 3;";
            return _dbConnection.GetConnection().Query<TransMC>(query);
        }
    }
}
