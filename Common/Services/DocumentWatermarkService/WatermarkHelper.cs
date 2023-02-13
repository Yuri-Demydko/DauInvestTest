using PdfSharpCore.Drawing;

namespace Common.Services.DocumentWatermarkService;

 public enum WatermarkLocation
    {
        BottomOne,
        BottomTwo,
        BottomTree,
        BottomFour,
        BottomFive,
        TopRight,
        TopLeft
    }

    public static class WatermarkHelper
    {
        public static readonly XFont SignFont = new XFont("Arial", 10);

        public static void SetImageWatermark(in XGraphics page, WatermarkLocation location, Stream imageStream,int imageHeight)
        {
            switch(location)
            {
                case WatermarkLocation.BottomOne:
                    page.DrawImage(XImage.FromStream(()=>imageStream),new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - imageHeight - 110));
                    break;
                case WatermarkLocation.BottomTwo:
                    page.DrawImage(XImage.FromStream(()=>imageStream),new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - imageHeight - 85));
                    break;
                case WatermarkLocation.BottomTree:
                    page.DrawImage(XImage.FromStream(()=>imageStream),new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - imageHeight - 60));
                    break;
                case WatermarkLocation.BottomFour:
                    page.DrawImage(XImage.FromStream(()=>imageStream),new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - imageHeight - 35));
                    break;
                case WatermarkLocation.BottomFive:
                    page.DrawImage(XImage.FromStream(()=>imageStream),new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - imageHeight - 10));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }
        }
        public static void SetTextWatermark(in XGraphics page, string watermark, XColor color, WatermarkLocation location,
            XFont font)
        {
            // Get the size (in points) of the text.
            var size = page.MeasureString(watermark, font);

            switch (location)
            {
                case WatermarkLocation.BottomOne:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(10,
                            page.PdfPage.Height - size.Height - 120, page.PdfPage.Width - 15,
                            size.Height + 5));
                        
                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - size.Height - 110),
                            XStringFormats.Center);
                        break;
                    }
                case WatermarkLocation.BottomTwo:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(10,
                            page.PdfPage.Height - size.Height - 95, page.PdfPage.Width - 15,
                            size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - size.Height - 85),
                            XStringFormats.Center);
                        break;
                    }
                case WatermarkLocation.BottomTree:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(10,
                            page.PdfPage.Height - size.Height - 70, page.PdfPage.Width - 15,
                            size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - size.Height - 60),
                            XStringFormats.Center);
                        break;
                    }
                case WatermarkLocation.BottomFour:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(10,
                            page.PdfPage.Height - size.Height - 45, page.PdfPage.Width - 15,
                            size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - size.Height - 35),
                            XStringFormats.Center);
                        break;
                    }
                case WatermarkLocation.BottomFive:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(10,
                            page.PdfPage.Height - size.Height - 20, page.PdfPage.Width - 15,
                            size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width / 2, page.PdfPage.Height - size.Height - 10),
                            XStringFormats.Center);
                        break;
                    }
                case WatermarkLocation.TopRight:
                    {
                        page.DrawRectangle(new XPen(color, 2), new XRect(page.PdfPage.Width - size.Width - 15,
                            size.Height - 5, size.Width + 10,
                            size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color),
                            new XPoint(page.PdfPage.Width - size.Width - 10, size.Height + 10),
                            XStringFormats.Default);
                        break;
                    }

                case WatermarkLocation.TopLeft:
                    {
                        page.DrawRectangle(new XPen(color, 2),
                            new XRect(15, size.Height - 5, size.Width + 10, size.Height + 5));

                        // Draw the string.
                        page.DrawString(watermark, font, new XSolidBrush(color), new XPoint(20, size.Height + 10),
                            XStringFormats.Default);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }
        }
    }