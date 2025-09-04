using Dapper;
using Newtonsoft.Json.Linq;
using System.Transactions;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Renting(IDatabaseConnectionSys dbConnection, ISMIMLogdb iSMIMLogdb, ISessionHelper sessionHelper) : IRenting
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;

        public async Task<IEnumerable<TransMC>> GetList()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [Supplier] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 0) AND FPTag=0 AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
        }

        public async Task<IEnumerable<TransMC>> GetListPayments()
        {
            string query = "SELECT [Id], [QrCode], [SerialNo], [MachineType], [CurrentUnit], [Supplier] FROM [SMIM_VwMcInventory] WHERE [CurrentStatus] IN (1,2) AND (IsOwned = 0) AND FPTag=1 AND CurrentUnitId IN @AccessPlants ORDER BY QrCode;";
            return await _dbConnection.GetConnection().QueryAsync<TransMC>(query, new { AccessPlants = _iSessionHelper.GetLocationList() });
        }

        public async Task<WorkCompCertificate> GetMachinesByIdsAsync(List<int> ids)
        {
            var oWorkCompCertificate = new WorkCompCertificate();

            using var connection = _dbConnection.GetConnection();

            // Fetch machine details
            const string machineQuery = @"";
            var certData = new WorkCompCertificate
            {
                Id = 0,
                InvoiceNumber = "INV-0001",
                SupplierId = 0,
                SupplierName = "Supplier Name",
                PaymentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                DeductionOfWork = "10%",
                DaysCount = "30",
                SysTotContaractSum = "10000",
                InvTotContractSum = "9000",
                InvAdvancePayment = "1000",
                InvDeductPayment = "500",
                InvTotalAmountPay = "7500",
                AmountInWords = "Seven Thousand Five Hundred Only",
                Remarks = "Remarks here"
            };

            // Fetch ApprovalOrg
            const string approvalOrgQuery = @"";

            var appOrgTmplate = new ApprovalOrgTemaplete
            {
               Id = 0,
                ActionByName = "Approval Organization",
                Designation = "Designation",
                ActionById = 1,
                ActionName = "John Doe",
                ActionOn = DateTime.Now.ToString("yyyy-MM-dd"),
                ActionStatus = 0
            };


            // Fetch WorkCompCertMcList
            const string WorkCompCertMcListQuery = @"";
            var WorkCompCertMcList = new List<WorkCompCertMc>();

            var OMc1 = new WorkCompCertMc
            {
                Id = 0,
                MachineSerialNo = "ABC",
                MachineType = "Type",
                MachineName = "MC Name",
                PerDayCost = "100"
            };

            WorkCompCertMcList.Add(OMc1);

            oWorkCompCertificate = certData;
            oWorkCompCertificate.ApprovalOrg = appOrgTmplate;
            oWorkCompCertificate.WorkCompCertMcList = WorkCompCertMcList;
            return oWorkCompCertificate;
        }

        public async Task<MachineRentedVM?> GetRentedMcById(int id)
        {
            const string query = @"
            SELECT Id, QrCode, SerialNo, FarCode, DateBorrow, DateDue, ServiceSeq, MachineBrand, MachineType, Location, OwnedUnit, CurrentUnit, MachineModel, Cost, ImageFR, ImageBK, Status, Supplier, CostMethod, Comments,
            LastScanDateTime, DispatchImageAv, ReturnGPImageAv FROM SMIM_VwMcInventory WHERE Id = @Id AND IsDelete = 0";

            return await _dbConnection.GetConnection().QuerySingleOrDefaultAsync<MachineRentedVM>(query, new { Id = id });
        }

        public async Task<string[]> UpdateStatus(string remarks, string id, bool action)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            string[] result = new string[2];
            DateTime nowDT = DateTime.Now;

            try
            {
              

                if (string.IsNullOrEmpty(id))
                {
                    result[0] = "0";
                    result[1] = "No record found for the given Id.";
                    return result;
                }

                int rowsAffected = 0;
                // Update machine transfer status
                string updateTransferQuery = @"UPDATE [dbo].[SMIM_TrInventory]
                   SET [FinanceApprove] = @FinanceApprove
                      ,[FinanceApproveRemarks] = @FinanceApproveRemarks
                      ,[FinanceAppBy] = @UserId
                      ,[FInanceAppDateTime] = @NowDT
                 WHERE   (Id = @McId)";
                rowsAffected += await connection.ExecuteAsync(updateTransferQuery, new { FinanceApprove = action? 1:2, FinanceApproveRemarks = remarks, NowDT = nowDT, UserId = _iSessionHelper.GetUserId(), McId = id }, transaction);

                Logdb logdb = new()
                {
                    TrObjectId = int.Parse(id),
                    TrLog = "RENT AGREEMENT " + (action ? "APPROVED" : "REJECTED")
                };

                _iSMIMLogdb.InsertLog(connection, logdb, transaction);

                transaction.Commit();
                // Set result based on whether rows were affected
                if (rowsAffected > 0)
                {
                    result[0] = "1";
                    result[1] = "Successfully Updated !";
                }
                else
                {
                    result[0] = "0";
                    result[1] = "No records were updated. The Id may not exist.";
                }
            }
            catch (Exception ex)
            {
                result[0] = "0";
                result[1] = ex.Message;
                transaction.Rollback();
            }

            return result;
        }
    }
}
