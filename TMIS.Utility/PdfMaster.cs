using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using QRCoder;
using Drw = System.Drawing;
using Img = System.Drawing.Imaging;
using Encoder = System.Drawing.Imaging.Encoder;


namespace TMIS.Utility
{
    public class PdfMaster
    {
        public struct TripleValuePair<TKey, TValue1, TValue2>
        {
            public TKey Key { get; }
            public TValue1 Value1 { get; }
            public TValue2 Value2 { get; }

            public TripleValuePair(TKey key, TValue1 value1, TValue2 value2)
            {
                Key = key;
                Value1 = value1;
                Value2 = value2;
            }
        }

        public static async Task<byte[]> GenerateQRCodeAsync(List<TripleValuePair<string, string, string>> qrCodes)
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

                                    // ==== NEW: 2-column table for company name + status ====
                                    PdfPTable headerTable = new PdfPTable(2);
                                    headerTable.WidthPercentage = 100;
                                    float[] headerWidths = { 0.6f, 0.3f }; // 70% | 30%
                                    headerTable.SetWidths(headerWidths);

                                    // Left: Company Name
                                    PdfPCell companyCell = new PdfPCell(
                                        new Phrase("TIMEX GARMENTS (PVT) LTD.", new Font(Font.HELVETICA, 8))
                                    )
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_LEFT,
                                    };
                                    headerTable.AddCell(companyCell);

                                    // Right: Status (white on black, rounded)
                                    PdfPCell statusCell = new PdfPCell(
                                     new Phrase(qrCodeValue.Value2,
                                         new Font(Font.HELVETICA, 8, Font.BOLD, BaseColor.White)))
                                    {
                                        BackgroundColor = BaseColor.Black,
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_RIGHT,
                                        VerticalAlignment = Element.ALIGN_TOP,
                                        PaddingRight = 20f
                                    };

                                    headerTable.AddCell(statusCell);

                                    // ==== Main inner table (1 column) ====
                                    PdfPTable innerTable = new PdfPTable(1);
                                    innerTable.WidthPercentage = 100;

                                    // Add the header (company + status)
                                    innerTable.AddCell(headerTable);

                                    // Add QR code
                                    PdfPCell qrCell = new PdfPCell(qrImage)
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        PaddingTop = 2f
                                    };
                                    innerTable.AddCell(qrCell);

                                    // Add QR code + serial (Value1)
                                    string showTxt = qrCodeValue.Key + " - [ " + qrCodeValue.Value1 + " ]";
                                    PdfPCell textCell = new PdfPCell(
                                        new Phrase(showTxt, new Font(Font.HELVETICA, 8))
                                    )
                                    {
                                        Border = Rectangle.NO_BORDER,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        VerticalAlignment = Element.ALIGN_BOTTOM
                                    };
                                    innerTable.AddCell(textCell);

                                    // ==== Final outer cell (with border) ====
                                    PdfPCell outerCell = new PdfPCell(innerTable)
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

                    // Pad with empty cells if odd count
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


        public static async Task<byte[]?> ImageToPdfAsync(IFormFile? uploadedImage)
        {
            if (uploadedImage == null || uploadedImage.Length == 0)
                return null;

            using var ms = new MemoryStream();
            using (var doc = new Document(PageSize.A4))
            {
                var writer = PdfWriter.GetInstance(doc, ms);
                writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

                doc.Open();

                // Compress image
                byte[] compressedBytes = await CompressImage(uploadedImage, 80L);
                var image = iTextSharp.text.Image.GetInstance(compressedBytes);

                image.ScaleToFit(doc.PageSize.Width - 10, doc.PageSize.Height - 10);
                image.Alignment = Element.ALIGN_CENTER;

                doc.Add(image);
                doc.Close();
            }

            return ms.ToArray();
        }

        private static async Task<byte[]> CompressImage(IFormFile uploadedImage, long quality = 75L)
        {
            using var inputStream = uploadedImage.OpenReadStream();
            using var bitmap = new Drw.Bitmap(inputStream);
            using var ms = new MemoryStream();

            var encoder = Img.ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == Img.ImageFormat.Jpeg.Guid);
            var encoderParams = new Img.EncoderParameters(1);
            encoderParams.Param[0] = new Img.EncoderParameter(Encoder.Quality, quality);
            bitmap.Save(ms, encoder, encoderParams);

            return await Task.FromResult(ms.ToArray());
        }
    }
}
