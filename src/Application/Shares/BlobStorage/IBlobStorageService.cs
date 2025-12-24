namespace Application;

public interface IBlobStorageService
{
    Task<IEnumerable<string>> GetFilesAsync(string container);
    Task<string> GetFileAsync(string container, string blobName);
    Task<Stream> GetFileStreamAsync(string container, string blobName);
    Task<string> UploadFileAsync(string containerName, IFormFile files);
    Task<List<string>> UploadFilesAsync(string container, List<IFormFile> file);
    Task<bool> FileExistsAsync(string container, string blobName);
    Task<string> GenerateFileAccessUrlAsync(string container, string blobName, TimeSpan validDuration);
    Task<bool> DeleteFileAsync(string container, string blobName);
    Task<bool> DeleteFileByUrlAsync(string fileUrl);
    Task DeleteAllFilesAsync(string container);
}
