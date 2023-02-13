namespace Common.Services.FileService;

public interface IFileService
    {
        Task PutFileAsync(Stream stream, string folder, string fileName);
        
        Task PutFileAsync(Stream stream, string path);

        Task<MemoryStream> GetFileAsync(string path);

        bool DeleteFile(string path);
    }