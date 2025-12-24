namespace Domain;

public class Bank
{
    public string code { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public List<Datum> data { get; set; } = [];
}
