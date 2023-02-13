using Common.Enums;
using ZXing;
using ZXing.Common;
using ZXing.PngWriter;

namespace Common.Services.QrCodeService;

public class QrCodeService
{
    private const int CodeWidth = 25;
    private const int CodeHeight = 25;
    public QrCodeGenerationResult GenerateQrCode(string data)
    {
        try
        {
            var barcodeWriter = new BarcodeWriterGeneric
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = CodeWidth,
                    Height = CodeHeight,
                    Margin = 3
                }
            };

            var res = barcodeWriter.Encode(data);
            var writer = new PngWriter();
            
            return new QrCodeGenerationResult()
            {
                QrCodeImageStream = writer.Write(res),
                Result = OperationResult.Success,
                PixelSize = (CodeWidth,CodeHeight)
            };
        }
        catch (Exception)
        {
            return new QrCodeGenerationResult()
            {
                QrCodeImageStream = null,
                Result = OperationResult.Fail
            };
        }
    }
    
}