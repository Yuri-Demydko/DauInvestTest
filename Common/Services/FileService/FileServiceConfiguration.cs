using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FileOptions = DocumentService.Services.FileService.FileOptions;


namespace Common.Services.FileService;

public static class VolumeFileServiceConfiguration
{
    public static void AddVolumeFileService(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Files");

        services.AddSingleton<IFileService, VolumeFileService>();

        services.Configure<FileOptions>(options);
    }
}