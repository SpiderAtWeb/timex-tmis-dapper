using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;

namespace TMIS.Utility
{
    public class GenerateQR
    {

        public static byte[] GenerateQRCode(List<string> qrCodes)
        {
            using (MemoryStream memoryStream = new())
            {
                var document = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Define a 2-column table (ID card width)
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
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeValue, QRCodeGenerator.ECCLevel.Q);
                            using (BitmapByteQRCode qrCode = new(qrCodeData))
                            {
                                byte[] qrCodeImage = qrCode.GetGraphic(20);

                                // Convert QR code to image
                                Image qrImage = Image.GetInstance(qrCodeImage);
                                qrImage.ScaleAbsolute(100f, 100f); // Scaled to fit inside ID card
                                qrImage.Alignment = Element.ALIGN_CENTER;

                                // Create inner table (1 column: image + text)
                                PdfPTable innerTable = new(1);

                                PdfPCell textCellLogo = new(new Phrase("TIMEX GARMENTS (PVT) LTD.", new Font(Font.HELVETICA, 8, Font.NORMAL)))
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

                                PdfPCell textCell = new(new Phrase(qrCodeValue, new Font(Font.HELVETICA, 8, Font.NORMAL)))
                                {
                                    Border = Rectangle.NO_BORDER,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };
                                innerTable.AddCell(textCell);     

                                // Outer cell sized like ID card
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

                // Fill empty cells to complete row if needed
                while (totalCells % 2 != 0)
                {
                    table.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
                    totalCells++;
                }

                // Add the table to the document
                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }
        }
    }
}
