namespace Contract;

public class MediaUpload
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = AppConstants.BlobContainer;
    public string? ContentType { get; set; }
    public long MaxBlobSize { get; set; } = AppConstants.MaxBlobSize;
    public MediaType Type { get; set; }
}
