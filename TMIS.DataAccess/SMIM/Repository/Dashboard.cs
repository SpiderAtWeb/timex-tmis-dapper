using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Dashboard(IDatabaseConnectionSys dbConnection) : IDashBoard
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<List<MachineStatus>> GetDashBoardData()
        {
            var resultSetQuery = @"
            SELECT 
                COUNT(CASE WHEN CurrentStatusId = 1 THEN 1 END) AS CountNew,
                COUNT(CASE WHEN CurrentStatusId = 2 THEN 1 END) AS CountIdle,
                COUNT(CASE WHEN CurrentStatusId = 3 THEN 1 END) AS CountRequested,
                COUNT(CASE WHEN CurrentStatusId = 7 THEN 1 END) AS CountInTransit,
                COUNT(CASE WHEN CurrentStatusId = 8 THEN 1 END) AS CountRepair,
                COUNT(CASE WHEN CurrentStatusId = 6 THEN 1 END) AS CountRunning,
	            COUNT(CASE WHEN CurrentStatusId = 9 THEN 1 END) AS CountDisposed,
	            COUNT(CASE WHEN CurrentStatusId = 10 THEN 1 END) AS CountTerminate
            FROM SMIM_TrMachineInventory";

            var resultSet = await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<IconData>(resultSetQuery);

            if (resultSet == null)
            {
                return [];
            }

            var statuses = new List<MachineStatus>
            {
                new() { Status = "New", Value = resultSet.CountNew },
                new() { Status = "Idle", Value = resultSet.CountIdle },
                new() { Status = "Requested", Value = resultSet.CountRequested },
                new() { Status = "In-Trasit", Value = resultSet.CountInTransit },
                new() { Status = "Repairing", Value = resultSet.CountRepair },
                new() { Status = "Running", Value = resultSet.CountRunning },
                new() { Status = "Disposed", Value = resultSet.CountDisposed },
                new() { Status = "Rent-Terminated", Value = resultSet.CountTerminate }
            };

            return statuses;
        }

        public async Task<IEnumerable<DashboardSummary>> GetSmryDataAsync(string ownedClusterId)
        {
            var query = @"
            SELECT 
                [MdCompanyUnits] AS Units,
                [MdMachineTypes] AS MachineType,
                [OwStatus] AS Status
            FROM [dbo].[VwDashBoardSmry] WHERE (OwnedClusterId = @OwnedClusterId)";

            return await _dbConnection.GetConnection().QueryAsync<DashboardSummary>(query, new { OwnedClusterId = ownedClusterId });
        }

        public async Task<IEnumerable<PivotDataVM>> GetPivotData(string ownedClusterId)
        {
            var query = @"
            SELECT CurrentUnit,
                  Location,
                  MachineType,
                    CASE 
                        WHEN IsOwned = 1 THEN 'Owned' 
                        ELSE 'Rented' 
                    END AS OwnershipStatus
                FROM VwPivotData
            WHERE        (OwnedClusterId = @OwnedClusterId) ORDER BY CurrentUnit, Location, MachineType";

            return await _dbConnection.GetConnection().QueryAsync<PivotDataVM>(query, new { OwnedClusterId = ownedClusterId });
        }

        public string[] GetTrLoggerData(string mcId)
        {
            // SQL query to retrieve data
            string sql = @"
                SELECT TrDateTime, TrLog, TrUser
                FROM dbo.SMIM_TrLogger
                WHERE McId = @McId
                ORDER BY id DESC";

            // Execute the query and map the result to TrLogger objects
            var logs = _dbConnection.GetConnection()
                            .Query<TrLogger>(sql, new { McId = mcId })
                            .ToList();

            // Format the data as [TrDateTime]-[TrLog]-[TrUser]
            var formattedLogs = logs.Select(log => $"[{log.TrDateTime:yyyy-MM-dd HH:mm:ss}] - [{log.TrLog}] - [{log.TrUser}]").ToArray();

            return formattedLogs;
        }

        // Method to fetch all inventory data
        public async Task<IEnumerable<InventoryItem>> GetAllInventoryData()
        {
            string query = @"
               SELECT [Id]
              ,[QrCode]
              ,[SerialNo]
              ,[FarCode]
              ,CASE WHEN IsOwned = 1 THEN 'Owned' ELSE 'Rented' END AS Ownership
              ,CONVERT(VARCHAR(10), [DatePurchased], 101) AS [DatePurchased]
              ,CONVERT(VARCHAR(10), [DateBorrow], 101) AS [DateBorrow]
              ,[DateDue]
              ,[ServiceSeq]
              ,[MachineBrand]
              ,[MachineType]
              ,[CompanyGroup]
              ,[Location]
              ,[OwnedCluster]
              ,[OwnedUnit]
              ,[CurrentUnit]
              ,[MachineModel]
              ,[Supplier]
              ,[CostMethod]
              ,[Cost]
              ,[Comments]
              ,[Status]
              ,[LastScanDateTime]
               FROM [dbo].[VwMcInventory] WHERE IsDelete = 0 ORDER BY [CurrentUnit] ASC,[LastScanDateTime] DESC, [QrCode] ASC";  // SQL query for paging

            var result = await _dbConnection.GetConnection().QueryAsync<InventoryItem>(query);
            return result;
        }

        public Task<IEnumerable<SelectListItem>> GetClusterDetails()
        {
            string query = "SELECT CAST(Id AS NVARCHAR) AS Value, PropName AS Text FROM  SMIM_MdCompanyClusters WHERE (IsDelete = 0) ORDER BY Id";
            return _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
        }



    }
}
