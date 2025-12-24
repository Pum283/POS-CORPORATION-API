namespace Domain;

public class Datum
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
    public string bin { get; set; } = string.Empty;
    public string shortName { get; set; } = string.Empty;
    public string logo { get; set; } = string.Empty;
    public int transferSupported { get; set; }
    public int lookupSupported { get; set; }
    public string short_name { get; set; } = string.Empty;
    public int support { get; set; }
    public int isTransfer { get; set; }
    public string swift_code { get; set; } = string.Empty;
}
