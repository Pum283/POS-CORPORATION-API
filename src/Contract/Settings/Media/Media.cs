namespace Contract;

public class Media
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; } = null;
}
