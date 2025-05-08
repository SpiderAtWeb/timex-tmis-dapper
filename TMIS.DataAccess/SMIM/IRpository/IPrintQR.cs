namespace TMIS.DataAccess.SMIM.IRpository
{
    public interface IPrintQR
    {
        Task<IEnumerable<string>> GetQrCode();
    }
}
