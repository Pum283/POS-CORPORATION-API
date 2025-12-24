namespace Contract;

public static class FileValidatorHelper
{
    private static readonly string[] _permittedExtensions = [".jpg", ".png", ".gif", ".bmp", ".webp"];
    private static readonly string[] _permittedMimeTypes = [ "image/png", "image/gif", "image/bmp", "image/webp"];

    public static bool IsValidExtension(IFormFile? file)
    {
        var extension = Path.GetExtension(file!.FileName).ToLowerInvariant();
        return _permittedExtensions.Contains(extension);
    }

    public static bool IsValidMimeType(IFormFile file)
    {
        return _permittedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
    }

    public static bool IsValidSize(IFormFile? file, long maxBytes)
    {
         if(file!.Length > 0 && file.Length <= maxBytes)
        {
            return true;
        }
        return false;
    }
}

