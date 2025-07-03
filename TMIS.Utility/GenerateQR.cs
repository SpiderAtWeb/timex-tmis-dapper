using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;

namespace TMIS.Utility
{
    public class GenerateQR
    {

        public static async Task<byte[]> GenerateQRCodeAsync(List<KeyValuePair<string, string>> qrCodes)
        {
            return await Task.Run(() =>
            {
                using (MemoryStream memoryStream = new())
                {
                    var document = new iTextSharp.text.Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    PdfPTable table = new(2)
                    {
                        TotalWidth = 500f,
                        LockedWidth = true,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    float[] columnWidths = [1f, 1f];
                    table.SetWidths(columnWidths);

                    int totalCells = 0;

                    foreach (var qrCodeValue in qrCodes)
                    {
                        try
                        {
                            using (QRCodeGenerator qrGenerator = new())
                            {
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeValue.Key, QRCodeGenerator.ECCLevel.Q);
                                using (BitmapByteQRCode qrCode = new(qrCodeData))
                                {
                                    byte[] qrCodeImage = qrCode.GetGraphic(20);

                                    Image qrImage = Image.GetInstance(qrCodeImage);
                                    qrImage.ScaleAbsolute(100f, 100f);
                                    qrImage.Alignment = Element.ALIGN_CENTER;

                                    PdfPTable innerTable = new(1);

                                    PdfPCell textCellLogo = new(new Phrase("TIMEX GARMENTS (PVT) LTD.", new Font(Font.HELVETICA, 8)))
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        PaddingBottom = 4f
                                    };
                                    innerTable.AddCell(textCellLogo);

                                    PdfPCell qrCell = new(qrImage)
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        PaddingBottom = 4f
                                    };
                                    innerTable.AddCell(qrCell);

                                    string showTxt = qrCodeValue.Key + " - [ " + qrCodeValue.Value + " ]";

                                    PdfPCell textCell = new(new Phrase(showTxt, new Font(Font.HELVETICA, 8)))
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_CENTER
                                    };
                                    innerTable.AddCell(textCell);

                                    PdfPCell outerCell = new(innerTable)
                                    {
                                        Border = Rectangle.BOX,
                                        Padding = 10,
                                        FixedHeight = 153f,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        VerticalAlignment = Element.ALIGN_MIDDLE
                                    };

                                    table.AddCell(outerCell);
                                    totalCells++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error generating QR code for '{qrCodeValue}': {ex.Message}");
                        }
                    }

                    while (totalCells % 2 != 0)
                    {
                        table.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
                        totalCells++;
                    }

                    document.Add(table);
                    document.Close();

                    return memoryStream.ToArray();
                }
            });
        }

    }
}
