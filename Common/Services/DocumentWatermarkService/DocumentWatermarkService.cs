using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using ZXing;
using ZXing.Common;
using ZXing.PngWriter;


namespace Common.Services.DocumentWatermarkService;

public class DocumentWatermarkService
{
    public MemoryStream AddSignedByWatermark(Stream item,string signedByData,WatermarkLocation location)
    {
        return AddWatermark(item, null, lastPage =>
            WatermarkHelper.SetTextWatermark(lastPage,
               signedByData+$" {DateTime.UtcNow.AddHours(3):g} +03:00",
                XColors.Green, location, WatermarkHelper.SignFont));
    }
    
    public MemoryStream AddQrCode(Stream item,Stream qrCodeStream, int height,WatermarkLocation location)
    {
        return AddWatermark(item, null, lastPage =>
            WatermarkHelper.SetImageWatermark(lastPage,location,qrCodeStream,height));
    }
    
    private  MemoryStream AddWatermark(Stream item, Action<XGraphics> watermark,
        Action<XGraphics> sign)
    {
        using var oldFile = PdfReader.Open(item, PdfDocumentOpenMode.Import);
        var newFile = new PdfDocument();

        foreach (var oldPage in oldFile.Pages)
        {
            var page = newFile.AddPage(oldPage);

            using var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

            watermark?.Invoke(gfx);
        }

        using var lastPageGfx = XGraphics.FromPdfPage(newFile.Pages[^1], XGraphicsPdfPageOptions.Append);
        sign?.Invoke(lastPageGfx);

        var pdfStream = new MemoryStream();
        newFile.Save(pdfStream);

        return pdfStream;
    }
}