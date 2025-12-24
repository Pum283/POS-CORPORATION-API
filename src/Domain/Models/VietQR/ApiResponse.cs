namespace Domain;

public class ApiResponse
{
    public string code { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public Data? data { get; set; }
}
public class Data
{
    public int acpId { get; set; }
    public string accountName { get;set; } = string.Empty;
    public string qrCode { get; set; } = string.Empty;
    public string qrDataUrl { get; set; } = string.Empty;
}
