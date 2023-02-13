using Common.Enums;

namespace Common.Services.QrCodeService;

public class QrCodeGenerationResult
{
    public Stream QrCodeImageStream { get; set; }
    
    public OperationResult Result { get; set; }
    
    public (int,int) PixelSize { get; set; }
}