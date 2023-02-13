

namespace Common.Services.CodeService;

public interface ICodeService
{
    public Task<CodeGenerationResult> GenerateCode(string phone);
}