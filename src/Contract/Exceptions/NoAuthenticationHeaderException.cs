namespace Contract;

public class NoAuthenticationHeaderException : DomainException
{
    public NoAuthenticationHeaderException() : base("Unauthorized", "There is no authentication header in this request")
    {
    }
}
