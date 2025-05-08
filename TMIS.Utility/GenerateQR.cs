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

                // Define a 4-column table for QR codes
                PdfPTable table = new(4)
                {
                    WidthPercentage = 100
                };
                float[] columnWidths = new float[] { 1f, 1f, 1f, 1f }; // Column widths to make the table spread across the page
                table.SetWidths(columnWidths);

                // Track the number of cells added to ensure we can fill the table properly
                int totalCells = 0;

                foreach (var qrCodeValue in qrCodes)
                {
                    try
                    {
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeValue, QRCodeGenerator.ECCLevel.Q);
                            using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
                            {
                                byte[] qrCodeImage = qrCode.GetGraphic(20);

                                // Create QR code image and scale it
                                Image qrImage = Image.GetInstance(qrCodeImage);
                                qrImage.ScaleAbsolute(100, 100); // Adjust size as needed

                                // Create a Phrase to combine QR code image and text
                                PdfPTable innerTable = new(1); // Inner table with a single column
                                PdfPCell qrImageCell = new(qrImage)
                                {
                                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    PaddingBottom = 1 // Space between QR code and text
                                };
                                innerTable.AddCell(qrImageCell);

                                PdfPCell textCell = new(new Phrase(qrCodeValue, new Font(Font.HELVETICA, 8, Font.NORMAL)))
                                {
                                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };
                                innerTable.AddCell(textCell);

                                // Add the combined inner table (QR code + text) to the main table
                                PdfPCell mainCell = new(innerTable)
                                {
                                    Border = iTextSharp.text.Rectangle.BOX,
                                    Padding = 2,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE
                                };
                                table.AddCell(mainCell);
                                totalCells++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error generating QR code for value '{qrCodeValue}': {ex.Message}");
                    }
                }

                // Fill empty cells to complete the table (if necessary)
                while (totalCells % 4 != 0)
                {
                    table.AddCell(new PdfPCell() { Border = iTextSharp.text.Rectangle.NO_BORDER });
                    totalCells++;
                }

                // Add the table to the document
                document.Add(table);
                document.Close();

                return memoryStream.ToArray(); // Return the PDF as a byte array
            }
        }




    }
}
