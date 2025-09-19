using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class SMApprovalService(IDatabaseConnectionSys dbConnection, IRenting renting) : ISMApprovalService
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IRenting _renting = renting;

        public async Task<int> SMUpdateAsync(string invoiceCode, int action)
        {
            try
            {
                using var connection = _dbConnection.GetConnection();
                using var transaction = connection.BeginTransaction();

                var invoiceParts = invoiceCode.Split('|');

                if (invoiceParts.Length < 3)
                    throw new ArgumentException("Invalid invoice code format.", nameof(invoiceCode));

                string id = invoiceParts[0];
                string level = invoiceParts[1];
                string levelNo = invoiceParts[2];

                // Only allow levels 2–6
                if (!int.TryParse(levelNo, out int levelIndex) || levelIndex < 2 || levelIndex > 6)
                    throw new ArgumentOutOfRangeException(nameof(invoiceCode), "Approval level Invalid");

                // Build SQL dynamically
                string sql = $@"
            UPDATE [dbo].[SMIM_TrRentPayments]
            SET   [AppLevelStat{levelIndex}] = @AppStatus,
                  [AppLevelStat{levelIndex}On] = GETDATE()
            WHERE (Id = @ID) AND (ApproveLevel{levelIndex}By = @Level)";

                var parameters = new
                {
                    ID = id,
                    Level = level,
                    AppStatus = action
                };

                // First update
                int rows = await connection.ExecuteAsync(sql, parameters, transaction);

                if (rows == 0)
                {
                    transaction.Rollback();
                    return 0; // nothing updated
                }

                transaction.Commit();

                // After commit, send mail if needed
                if (action == 1 && levelIndex <= 5)
                {
                    await _renting.PrepairEmail(int.Parse(id), levelIndex + 1, connection, null);
                }

                return 1;
            }
            catch
            {
                return -1;
                throw;
            }
        }


        public async Task<(string, string)> GetSMInfoAsync(string invoiceCode)
        {
            var invoiceParts = invoiceCode.Split('|');

            if (invoiceParts.Length < 3)
                throw new ArgumentException("Invalid invoice code format.", nameof(invoiceCode));

            string id = invoiceParts[0];
            string levelNo = invoiceParts[2];

            if (!int.TryParse(levelNo, out int levelIndex) || levelIndex < 2 || levelIndex > 6)
                throw new ArgumentOutOfRangeException(nameof(invoiceCode), "Approval level Invalid");

            // Build dynamic column
            string sql = $@"
            SELECT [AppLevelStat{levelIndex}] AS Status, InvoiceNo
            FROM [dbo].[SMIM_VwPaymentHeader]
            WHERE Id = @ID";

            var result = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<(string Status, string InvoiceNo)>(sql, new { ID = id });
            return (result.Status, result.InvoiceNo);
        }


    }
}
