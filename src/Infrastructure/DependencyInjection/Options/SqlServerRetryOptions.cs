namespace Infrastructure;

public class SqlServerRetryOptions
{
    public int MaxRetryCount { get; set; }
    public TimeSpan MaxRetryDelay { get; set; }
    public List<int> ErrorNumbersToAdd { get; set; } = new();
}
