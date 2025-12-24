using Microsoft.AspNetCore.Http;

namespace Contract;

public class ConvertMediaHelper
{
    public static List<IFormFile>? ConvertMediasToFormFiles(List<MediaUpload>? medias)
    {
        if (medias == null || medias.Count == 0)
            return null;

        var formFiles = new List<IFormFile>();

        foreach (var media in medias)
        {
            var stream = media.FileStream;
            var fileName = media.FileName ?? string.Empty;

            if (stream != null)
            {
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName);
                formFiles.Add(formFile);
            }
        }

        return formFiles;
    }

    public static List<MediaUpload> ConvertFormFilesToMedias(List<IFormFile>? formFiles)
    {
        if (formFiles == null || formFiles.Count == 0)
            return [];

        return formFiles.Select(file => new MediaUpload
        {
            FileName = file.FileName,
            FileStream = file.OpenReadStream(),
            Type = GetMediaType(file.ContentType),
        }).ToList();
    }

    public static IFormFile? ConvertMediaToFormFile(MediaUpload? media)
    {
        if (media?.FileStream == null)
            return null;

        return new FormFile(media.FileStream, 0, media.FileStream.Length, "file", media.FileName ?? string.Empty);
    }

    public static MediaUpload? ConvertFormFileToMedia(IFormFile? formFile)
    {
        if (formFile == null)
            return null;

        return new MediaUpload
        {
            FileName = formFile.FileName,
            FileStream = formFile.OpenReadStream(),
            Type = GetMediaType(formFile.ContentType),
        };
    }

    public static MediaType GetMediaType(string contentType)
    {
        if (contentType.StartsWith("image/"))
            return MediaType.Image;
        if (contentType.StartsWith("video/"))
            return MediaType.Video;
        if (contentType.StartsWith("file/"))
            return MediaType.File;
        if (contentType.StartsWith("audio/"))
            return MediaType.Audio;
        return MediaType.Text;
    }

    public static MediaType GetMediaTypeFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return MediaType.Text;

        var extension = Path.GetExtension(url)?.ToLower();

        return extension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => MediaType.Image,
            ".mp4" or ".mkv" or ".mov" or ".avi" or ".wmv" or ".flv" => MediaType.Video,
            ".mp3" or ".m4a" or ".wav" or ".aac" or ".flac" or ".ogg" => MediaType.Audio,
            ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx" => MediaType.File,
            _ => MediaType.Text
        };
    }
}
