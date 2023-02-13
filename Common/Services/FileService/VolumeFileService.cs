using System.Reflection;
using DocumentService.Services.FileService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FileOptions = DocumentService.Services.FileService.FileOptions;

namespace Common.Services.FileService;

public class VolumeFileService:IFileService
{
    private readonly FileOptions options;
    private readonly ILogger<VolumeFileService> logger;

    public VolumeFileService(IOptions<FileOptions> options, ILogger<VolumeFileService> logger)
    {
        this.logger = logger;
        this.options = options.Value;
            
        // create all directories if no exist
        foreach (var directory in typeof(Folders).GetFields(BindingFlags.Public | BindingFlags.Static |
                                                            BindingFlags.FlattenHierarchy)
                     .Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(i => (string)i.GetRawConstantValue()))
        {
            if (!Directory.Exists(this.options.Folder + "/" + directory))
            {
                Directory.CreateDirectory(this.options.Folder + "/" + directory);
            }
        }
    }

    public async Task PutFileAsync(Stream stream, string folder, string fileName)
    {
        await PutFileAsync(stream, folder + "/" + fileName);
    }

    public async Task PutFileAsync(Stream stream, string path)
    {
        await using var file = File.Create(options.Folder + "/" + path);
        await stream.CopyToAsync(file);
    }

    public async Task<MemoryStream> GetFileAsync(string path)
    {
        var file = await File.ReadAllBytesAsync(options.Folder + "/" + path);

        return new MemoryStream(file, 0, file.Length, false, true);
    }

    public bool DeleteFile(string path)
    {
        var exists = File.Exists(path);
        if(exists)
        {
            File.Delete(path);
        }

        return exists;
    }
}