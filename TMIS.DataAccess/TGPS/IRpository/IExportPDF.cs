using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.DataAccess.TGPS.IRpository
{
    public interface IExportPDF
    {
        public Task<(byte[] PdfBytes, string Message)> DownloadPdf(int reportId);
    }
}
