namespace Common.Services.CodeService;

public class CodeServiceMock:ICodeService
{
    public Task<CodeGenerationResult> GenerateCode(string phone)
        => Task.FromResult(new CodeGenerationResult()
        {
            Code = new Random().Next(0, 10000).ToString("0000"),
            Result = Enums.OperationResult.Success
        });
}