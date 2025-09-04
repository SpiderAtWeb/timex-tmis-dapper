using Dapper;
using System.Text.RegularExpressions;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.Utility;

namespace TMIS.DataAccess.SMIM.Repository
{
    public class PrintQR(IDatabaseConnectionSys dbConnection) : IPrintQR
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<string>> GetQrCode()
        {
            string query = @"SELECT QrCode +' - [ '+ SerialNo +' ] '+ 
            CASE
            WHEN  IsOwned  = 1 THEN 'TIMEX'
            ELSE 'RENTED' END AS STS
            FROM SMIM_TrInventory WHERE (IsDelete = 0) ORDER BY QrCode";
            var result = await _dbConnection.GetConnection().QueryAsync<string>(query);
            return result;
        }

        public async Task<byte[]> GetQrCodesPrint(List<string> qrCodes)
        {
            List<PdfMaster.TripleValuePair<string, string, string>> oQrCodes = [.. qrCodes
            .Select(r =>
            {
                var match = Regex.Match(r, @"^(.*?)\s*-\s*\[\s*(.*?)\s*\]\s*(.*)$");
                if (match.Success)
                {
                    string qrCode = match.Groups[1].Value.Trim();   // e.g. "TSM00001"
                    string serial = match.Groups[2].Value.Trim();    // e.g. "FD25791"
                    string status = match.Groups[3].Value.Trim();


                    return new PdfMaster.TripleValuePair<string, string, string>(qrCode, serial, status);
                }
                else
                {
                    return new PdfMaster.TripleValuePair<string, string, string>(r, "", ""); // fallback if format unexpected
                }
            })];

            return await PdfMaster.GenerateQRCodeAsync(oQrCodes);
        }
    }
}
