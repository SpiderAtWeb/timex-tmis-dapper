using Dapper;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;
using TMIS.Utility;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class Renting(IDatabaseConnectionSys dbConnection,
        ISMIMLogdb iSMIMLogdb,
        ISessionHelper sessionHelper,
        IUserControls userControls,
        ISMIMCommon sMIMCommon,
        IGmailSender gmailSender) : IRenting
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ISMIMLogdb _iSMIMLogdb = iSMIMLogdb;
        private readonly ISessionHelper _iSessionHelper = sessionHelper;
        private readonly ISMIMCommon _sMIMCommon = sMIMCommon;
        private readonly IUserControls _userControls = userControls;
        private readonly IGmailSender _gmailSender = gmailSender;

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

        public async Task<WorkCompCertificate?> GetMachinesByIdsAsync(List<int> ids)
        {
            using var connection = _dbConnection.GetConnection();

            const string machineQuery = @"
            SELECT Id, QrCode, SerialNo, MachineType,  CAST(Cost AS VARCHAR(20)) + '/' + CostMethod AS CostDisplay, Supplier, (CAST(ROUND(Cost / DaysCount, 2) AS DECIMAL(18,2))) AS PerDayCost
            FROM SMIM_VwMcInventory
            WHERE IsOwned = 0 AND Id IN @Ids";

            var machines = await connection.QueryAsync<WorkCompCertMc>(
                machineQuery,
                new { Ids = ids }
            );

            var unitsList = await _sMIMCommon.GetUnitsList();
            var approversList = await GetUnitsApprovers(connection);
            var supplierList = await _userControls.LoadDropDownsAsync("SMIM_MasterTwoRentSuppliers");

            return new WorkCompCertificate
            {
                WorkCompCertMcList = [.. machines],
                UnitsList = unitsList,
                ApprovalCat = approversList,
                SupplierList = supplierList
            };
        }

        private async Task<IEnumerable<SelectListItem>> GetUnitsApprovers(IDbConnection connection)
        {
            var query = @"SELECT Id, UserEmail AS PropName
            FROM ADMIN.dbo._MasterUsers AS A WHERE (IsActive = 1) ORDER BY PropName";

            var units = await connection.QueryAsync(query);
            return units.Select(unit => new SelectListItem
            {
                Value = unit.Id.ToString(),
                Text = unit.PropName
            });
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
                rowsAffected += await connection.ExecuteAsync(updateTransferQuery, new { FinanceApprove = action ? 1 : 2, FinanceApproveRemarks = remarks, NowDT = nowDT, UserId = _iSessionHelper.GetUserId(), McId = id }, transaction);

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

        public async Task<(byte[] PdfBytes, string Message)> GetGenerateVoucher(WorkCompCertificate opWorkComp, IFormFile supplierInvoiceImage)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {

                byte[]? _imgInvoice = null;

                if (supplierInvoiceImage != null && supplierInvoiceImage.Length > 0)
                {
                    _imgInvoice = await PdfMaster.ImageToPdfAsync(supplierInvoiceImage);
                }
      
                // Generate invoice number
                var invoiceNo = await _userControls.GenerateRefAsync(
                    connection, transaction, "[SMIM_XysGenerateNumber]", "SMINV");

                // Days count
                var daysCount = (Convert.ToDateTime(opWorkComp.ToDate) -
                                 Convert.ToDateTime(opWorkComp.FromDate)).Days;

                // Calculate total cost
                var totalCost = opWorkComp.WorkCompCertMcList.Sum(item => item.PerDayCost * daysCount);

                // Insert Master record (return new Id)
                var queryM = @"
            INSERT INTO [dbo].[SMIM_TrRentPayments]
                ([RaisedDate],[InvoiceNo],[UnitId],[InvoiceFromDate],[InvoiceToDate],[DaysCount],[VendorInvoiceImage],
                 [CertificateRemarks],[SystemCalculatedSum],[VendorContractSum],[VendorAdvancePayment],[VendorTotalAmount],
                 [GeneratedBy],[GeneratedOn],[ApproveLevel2By],[AppLevelStat2],[ApproveLevel3By],[AppLevelStat3],
                 [ApproveLevel4By],[AppLevelStat4],[ApproveLevel5By],[AppLevelStat5],[ApproveLevel6By],[AppLevelStat6],[TaskStart],[TaskComplete], [SupplierId])
            VALUES
                (@NowDT,@InvoiceNo,@UnitId,@InvoiceFromDate,@InvoiceToDate,@DaysCount, @VendorInvoiceImage,
                 @CertificateRemarks,@SystemCalculatedSum,@VendorContractSum,@VendorAdvancePayment,@VendorTotalAmount,
                 @GeneratedBy,@GeneratedOn,@ApproveLevel2By,0,@ApproveLevel3By,0,@ApproveLevel4By,0,@ApproveLevel5By,0,@ApproveLevel6By,0 ,0 ,0, @SupplierId);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                var nowDt = DateTime.Now;
                var insertedId = await connection.ExecuteScalarAsync<int>(
                    queryM,
                    new
                    {
                        VendorInvoiceImage = _imgInvoice,
                        InvoiceNo = invoiceNo,
                        opWorkComp.UnitId,
                        opWorkComp.SupplierId,
                        InvoiceFromDate = opWorkComp.FromDate,
                        InvoiceToDate = opWorkComp.ToDate,
                        DaysCount = daysCount,
                        CertificateRemarks = opWorkComp.Remarks,
                        SystemCalculatedSum = totalCost,
                        VendorContractSum = opWorkComp.InvContractSum,
                        VendorAdvancePayment = opWorkComp.InvAdvancePayment,
                        VendorTotalAmount = opWorkComp.InvTotalAmountPay,
                        GeneratedBy = _iSessionHelper.GetUserId(),
                        GeneratedOn = nowDt,
                        ApproveLevel2By = opWorkComp.FactoryAcctId,
                        ApproveLevel3By = opWorkComp.GeneralMgrId,
                        ApproveLevel4By = opWorkComp.GroupEngId,
                        ApproveLevel5By = opWorkComp.AppLevel1Id,
                        ApproveLevel6By = opWorkComp.AppLevel2Id,
                        NowDT = nowDt
                    },
                    transaction);

                // Insert child records
                var queryS = @"
            INSERT INTO [dbo].[SMIM_TrRentPaymentsAssets]
                ([InvoiceId],[AssetID],[PerDayCost])
            VALUES (@InvoiceId, @AssetID, @PerDayCost);";

                foreach (var mc in opWorkComp.WorkCompCertMcList)
                {
                    await connection.ExecuteAsync(
                        queryS,
                        new { InvoiceId = insertedId, AssetID = mc.Id, mc.PerDayCost },
                        transaction);

                    // Logging
                    Logdb logdb = new()
                    {
                        TrObjectId = mc.Id,
                        TrLog = "INVOICE CREATED - " + invoiceNo
                    };

                    _iSMIMLogdb.InsertLog(connection, logdb, transaction);
                }

                transaction.Commit();

                return await DownloadPdf(insertedId, invoiceNo);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<(byte[] PdfBytes, string Message)> DownloadPdf(int invoiceId, string invoiceNo) // Updated to match the interface
        {
            byte[] pdfBytes = []; // Initialize with an empty array to avoid nullability issues

            try
            {
                // Create report instance
                using Report report = new();
                // Load report template
                string reportPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Reports", "sm-invoice.frx");
                report.Load(reportPath);

                // Prepare data (example with DataSet)
                var dataSet = await GetReportData(invoiceId);

                // Register whole DataSet (optional)
                report.RegisterData(dataSet, "Data");

                // Register GatepassDetails table — this is the key line!
                report.RegisterData(dataSet.Tables["InvoiceMasterData"], "InvoiceMasterData");

                foreach (DataTable table in dataSet.Tables)
                {
                    report.GetDataSource(table.TableName).Enabled = true;
                }
                // Prepare report
                report.Prepare();

                var invoiceNoRef = invoiceNo.ToString();
                var safeFileName = invoiceNoRef.Replace("/", "-") ?? string.Empty; // Ensure non-null string

                // Export to PDF
                using (var pdfExport = new PDFSimpleExport())
                {
                    using (var stream = new MemoryStream())
                    {
                        pdfExport.Export(report, stream);
                        pdfBytes = stream.ToArray();
                        return (pdfBytes, safeFileName);
                    }
                }
            }
            catch
            {
                return (pdfBytes, string.Empty); // Ensure non-null string
            }
        }

        private async Task<DataSet> GetReportData(int invoiceId)
        {
            var dataSet = new DataSet();

            using var connection = _dbConnection.GetConnection();

            // First query
            using (var readerData = await connection.ExecuteReaderAsync(@"SELECT Supplier, RaisedDate, InvoiceNo, Unit, InvoiceFromDate, InvoiceToDate, DaysCount, CertificateRemarks, SystemCalculatedSum, VendorContractSum, VendorAdvancePayment, VendorTotalAmount, PreparedBy, 
             GeneratedOn, ApproveLevel2By, AppLevelStat2, AppLevelStat2On, ApproveLevel3By, AppLevelStat3, AppLevelStat3On, ApproveLevel4By, AppLevelStat4, AppLevelStat4On, ApproveLevel5By, AppLevelStat5, 
             AppLevelStat5On, ApproveLevel6By, AppLevelStat6, AppLevelStat6On, QrCode, SerialNo, MachineType, CostDisplay, PerDayCost
             FROM SMIM_VwPaymentExport WHERE (Id = @InvoiceId)", new { InvoiceId = invoiceId }))
            {
                var tableData = new DataTable("InvoiceMasterData");
                tableData.Load(readerData);
                dataSet.Tables.Add(tableData);
            }

            return dataSet;
        }

        public async Task<IEnumerable<PaymentsVM>> GetPaymentReadyList()
        {
            string query = @"SELECT        Id, InvoiceNo, FORMAT(RaisedDate, 'dd/MM/yyyy') AS RaisedDate, Supplier, VendorTotalAmount
            FROM            dbo.SMIM_VwPaymentHeader
            WHERE (TaskComplete = N'0') AND (GeneratedBy = @PreparedById)";

            return await _dbConnection.GetConnection().QueryAsync<PaymentsVM>(query, new { PreparedById = _iSessionHelper.GetUserId() });
        }

        public async Task<WorkCompCertificateVM> GetCertificateData(int id)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();

                // Query header and machine details in a single roundtrip
                var sql = @"
                SELECT Id,
                FORMAT(RaisedDate, 'dd/MM/yyyy') AS RaisedDate, 
                InvoiceNo, Unit, 
                FORMAT(InvoiceFromDate, 'dd/MM/yyyy') AS InvoiceFromDate, 
                FORMAT(InvoiceToDate, 'dd/MM/yyyy') AS InvoiceToDate, DaysCount, CertificateRemarks,
                       SystemCalculatedSum, VendorContractSum, VendorAdvancePayment, VendorTotalAmount,
                       PreparedBy, GeneratedOn,
                       ApproveLevel2By, AppLevelStat2, AppLevelStat2On,
                       ApproveLevel3By, AppLevelStat3, AppLevelStat3On,
                       ApproveLevel4By, AppLevelStat4, AppLevelStat4On,
                       ApproveLevel5By, AppLevelStat5, AppLevelStat5On,
                       ApproveLevel6By, AppLevelStat6, AppLevelStat6On, TaskStart, TaskComplete, Supplier
                FROM SMIM_VwPaymentHeader
                WHERE Id = @Id;

                SELECT QrCode, SerialNo, MachineType, CostDisplay, PerDayCost
                FROM SMIM_VwPaymentMachines
                WHERE InvoiceId = @Id;
            ";

                using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });

                var mainData = await multi.ReadFirstOrDefaultAsync<WorkCompCertificateVM>();
                if (mainData == null)
                    return new WorkCompCertificateVM { WorkCompCertMcList = [] };

                mainData.WorkCompCertMcList = (await multi.ReadAsync<WorkCompCertMc>()).ToList();
                return mainData;

            }
            catch
            {
                return new WorkCompCertificateVM { WorkCompCertMcList = [] };
                throw;
            }
        }

        public async Task<string> StartStarApproval(int id)
        {
            using var connection = _dbConnection.GetConnection();
            using var transaction = connection.BeginTransaction();

            var queryUpdate = @"UPDATE [dbo].[SMIM_TrRentPayments]
             SET   [TaskStart] = 1
                  ,[ProcessStartDate] = @Now
             WHERE [Id] = @ID";

            try
            {
                //Update TrMachineInventory status
                await _dbConnection.GetConnection().ExecuteAsync(queryUpdate, new
                {
                    ID = id,
                    DateTime.Now,
                }, transaction);


                await PrepairEmail(id, 2, connection, transaction);

                transaction.Commit();

                return "Successfully Started Approval Process";
            }
            catch
            {
                // Rollback the transaction if any command fails            
                transaction.Rollback();
                return "Something Went Wrong !!!";
                throw;
            }
        }

        public async Task PrepairEmail(int id, int levelIndex, IDbConnection connection, IDbTransaction? transaction)
        {

            // Query header and machine details in a single roundtrip
            var sql = @"
                SELECT Id, Supplier,
                FORMAT(RaisedDate, 'dd/MM/yyyy') AS RaisedDate, 
                InvoiceNo, Unit, 
                FORMAT(InvoiceFromDate, 'dd/MM/yyyy') AS InvoiceFromDate, 
                FORMAT(InvoiceToDate, 'dd/MM/yyyy') AS InvoiceToDate, DaysCount, CertificateRemarks,
                       SystemCalculatedSum, VendorContractSum, VendorAdvancePayment, VendorTotalAmount,
                       PreparedBy, GeneratedOn,
                       ApproveLevel2By, AppLevelStat2, AppLevelStat2On,
                       ApproveLevel3By, AppLevelStat3, AppLevelStat3On,
                       ApproveLevel4By, AppLevelStat4, AppLevelStat4On,
                       ApproveLevel5By, AppLevelStat5, AppLevelStat5On,
                       ApproveLevel6By, AppLevelStat6, AppLevelStat6On, TaskComplete, ProcessStartDate
                FROM SMIM_VwPaymentHeader
                WHERE Id = @Id;

                SELECT QrCode, SerialNo, MachineType, CostDisplay, PerDayCost
                FROM SMIM_VwPaymentMachines
                WHERE InvoiceId = @Id;
            ";

            using var multi = await connection.QueryMultipleAsync(sql, new { Id = id }, transaction);

            var header = await multi.ReadFirstOrDefaultAsync<WorkCompCertificateVM>();
            var details = (await multi.ReadAsync<WorkCompCertMc>()).ToList();

            // Prepare header part of array
            var myList = new List<string>
            {
                $"{header!.Id}",
                $"{header.InvoiceNo}",
                $"{header.RaisedDate}",
                $"{header.Supplier}",
                $"{header.Unit}",
                $"{header.InvoiceFromDate}",
                $"{header.InvoiceToDate}",
                $"{header.DaysCount}",
                $"{header.CertificateRemarks}",
                $"{header.SystemCalculatedSum}",
                $"{header.VendorContractSum}",
                $"{header.VendorAdvancePayment}",
                $"{header.VendorTotalAmount}",
                $"{header.PreparedBy}",
                $"{header.GeneratedOn}",
                $"{header.ApproveLevel2By}",
                $"{header.AppLevelStat2}",
                $"{header.AppLevelStat2On}",
                $"{header.ApproveLevel3By}",
                $"{header.AppLevelStat3}",
                $"{header.AppLevelStat3On}",
                $"{header.ApproveLevel4By}",
                $"{header.AppLevelStat4}",
                $"{header.AppLevelStat4On}",
                $"{header.ApproveLevel5By}",
                $"{header.AppLevelStat5}",
                $"{header.AppLevelStat5On}",
                $"{header.ApproveLevel6By}",
                $"{header.AppLevelStat6}",
                $"{header.AppLevelStat6On}",
                $"{header.ProcessStartDate}",
            };

            // Append each detail row as a string item in the array
            foreach (var d in details)
            {
                string detailString = $"{d.QrCode}|{d.SerialNo}|{d.MachineType}|{d.CostDisplay}|{d.PerDayCost}";
                myList.Add(detailString);
            }

            // Convert to array
            string[] myArray = [.. myList];

            var result = connection.QueryFirstOrDefault<(string ApproveById, string Email)>(
                $@"SELECT SMIM_TrRentPayments.ApproveLevel{levelIndex}By, 
             ADMIN.dbo._MasterUsers.UserEmail
              FROM SMIM_TrRentPayments 
              INNER JOIN ADMIN.dbo._MasterUsers 
                  ON SMIM_TrRentPayments.ApproveLevel{levelIndex}By = ADMIN.dbo._MasterUsers.Id
              WHERE SMIM_TrRentPayments.Id = @Id",
                new { Id = id }, transaction);

            if (result == default)
                throw new InvalidOperationException("No email found for the approved user.");

            // Send email
            await Task.Run(() => _gmailSender.SMInvoiceApproval(result.ApproveById, result.Email, levelIndex, myArray));
        }

        public async Task<byte[]> GetSupInvoicePdf(int invoiceId)
        {
            using var connection = _dbConnection.GetConnection();
            string sql = @"SELECT VendorInvoiceImage
                   FROM SMIM_TrRentPayments WHERE (Id = @Id)";
            var result = await connection.QueryFirstOrDefaultAsync<byte[]>(sql, new { Id = invoiceId });
            return result ?? [];
        }

        public async Task<byte[]> GetVoucherPdf(int invoiceId)
        {
            byte[] pdfBytes = []; // Initialize with an empty array to avoid nullability issues

            try
            {
                // Create report instance
                using Report report = new();
                // Load report template
                string reportPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Reports", "sm-invoice.frx");
                report.Load(reportPath);

                // Prepare data (example with DataSet)
                var dataSet = await GetReportData(invoiceId);

                // Register whole DataSet (optional)
                report.RegisterData(dataSet, "Data");

                // Register GatepassDetails table — this is the key line!
                report.RegisterData(dataSet.Tables["InvoiceMasterData"], "InvoiceMasterData");

                foreach (DataTable table in dataSet.Tables)
                {
                    report.GetDataSource(table.TableName).Enabled = true;
                }
                // Prepare report
                report.Prepare();

                // Export to PDF
                using (var pdfExport = new PDFSimpleExport())
                {
                    using (var stream = new MemoryStream())
                    {
                        pdfExport.Export(report, stream);
                        pdfBytes = stream.ToArray();
                        return pdfBytes;
                    }
                }
            }
            catch
            {
                return pdfBytes; // Ensure non-null string
            }
        }
    }
}
