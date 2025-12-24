namespace Domain;

public class ApiRequest
{
    public long accountNo { get; set; }
    public string accountName { get; set; } = string.Empty;
    public int acqId { get; set; }
    public int amount { get; set; }
    public string addInfo { get; set; } = string.Empty;
    public string format { get; set; } = string.Empty;
    public string template { get; set; } = string.Empty;
}
