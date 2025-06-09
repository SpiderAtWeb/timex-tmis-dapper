using Dapper;
using FastReport;
using FastReport.Export.PdfSimple;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.TGPS.IRpository;

namespace TMIS.DataAccess.TGPS.Rpository
{
    public class ExportPDF(IDatabaseConnectionSys dbConnection) : IExportPDF
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public (byte[] PdfBytes, string Message) DownloadPdf(int reportId) // Updated to match the interface
        {
            byte[] pdfBytes = []; // Initialize with an empty array to avoid nullability issues

            try
            {
                // Create report instance
                using Report report = new();
                // Load report template
                string reportPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "Reports", "gp-report.frx");
                report.Load(reportPath);

                // Prepare data (example with DataSet)
                var dataSet = GetReportData(reportId);

                // Register whole DataSet (optional)
                report.RegisterData(dataSet, "Data");

                // Register GatepassDetails table — this is the key line!
                report.RegisterData(dataSet.Tables["GatepassDetails"], "GatepassDetails");
                report.RegisterData(dataSet.Tables["GatepassRoutes"], "GatepassRoutes");

                foreach (DataTable table in dataSet.Tables)
                {
                    report.GetDataSource(table.TableName).Enabled = true;
                }
                // Prepare report
                report.Prepare();

                var gatepassRef = dataSet.Tables["GatepassDetails"]?.Rows.Cast<DataRow>().FirstOrDefault()?["GGpReference"]?.ToString();
                var safeFileName = gatepassRef?.Replace("/", "-") ?? string.Empty; // Ensure non-null string

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

        private System.Data.DataSet GetReportData(int gatepassId)
        {
            var dataSet = new DataSet();

            using var connection = _dbConnection.GetConnection();

            // First query
            using (var readerData = connection.ExecuteReader(@"
                 SELECT 
                     ID, 
                     GGpReference,
                     GpSubject, 
                     GeneratedUser, 
                     GeneratedDateTime, 
                     Attention, 
                     GGPRemarks, 
                     ApprovedBy, 
                     ApprovedDateTime, 
                     ItemName, 
                     ItemDescription, 
                     Quantity, 
                     UOM, 
                     IsApproved, 
                     BoiGatepass
                 FROM [TMIS].[dbo].[TGPS_VwGatePassList] 
                 WHERE ID = @GatepassId", new { GatepassId = gatepassId }))
            {
                var tableData = new DataTable("GatepassDetails");
                tableData.Load(readerData);
                dataSet.Tables.Add(tableData);
            }

            // Second query
            using (var readerRoutes = connection.ExecuteReader(@"
                 SELECT 
                     BusinessName, 
                     Address
                 FROM TGPS_VwGGPLocations 
                 WHERE GGpPassId = @GatepassId", new { GatepassId = gatepassId }))
            {
                var tableRoutes = new DataTable("GatepassRoutes");
                tableRoutes.Load(readerRoutes);
                dataSet.Tables.Add(tableRoutes);
            }

            return dataSet;
        }
    }
}
