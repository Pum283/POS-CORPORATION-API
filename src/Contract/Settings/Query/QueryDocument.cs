namespace Contract;

public class QueryDocument
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public TransType? TransType { get; set; }
}
