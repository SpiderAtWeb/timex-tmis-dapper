using Microsoft.AspNetCore.Http;
using System.Data;
using TMIS.Models.SMIS;
using TMIS.Models.SMIS.VM;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IRenting
    {
        Task<IEnumerable<TransMC>> GetList();

        Task<IEnumerable<TransMC>> GetListPayments();

        Task<MachineRentedVM?> GetRentedMcById(int id);

        Task<string[]> UpdateStatus(string remarks, string id, bool action);

        Task<WorkCompCertificate?> GetMachinesByIdsAsync(List<int> ids);

        Task<(byte[] PdfBytes, string Message)> GetGenerateVoucher(WorkCompCertificate opWorkComp, IFormFile supplierInvoiceImage);

        Task<IEnumerable<PaymentsVM>> GetPaymentReadyList();

        Task<WorkCompCertificateVM> GetCertificateData(int id);

        Task<string> StartStarApproval(int id);

        Task PrepairEmail(int id, int levelIndex, IDbConnection connection, IDbTransaction? transaction = null);

        Task<byte[]> GetVoucherPdf(int invoiceId);

        Task<byte[]> GetSupInvoicePdf(int invoiceId);


    }
}
