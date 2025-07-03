using TMIS.Models.SMIS;

namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IPrintQR
    {
        Task<IEnumerable<string>> GetQrCode();

        Task<byte[]> GetQrCodesPrint(List<string> qrCodes);
    }
}
