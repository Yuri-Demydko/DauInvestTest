namespace Common.Services.CodeService;

public class CodeGenerationResult
{
    public Enums.OperationResult Result { get; set; }
    
    public string? Code { get; set; }
}