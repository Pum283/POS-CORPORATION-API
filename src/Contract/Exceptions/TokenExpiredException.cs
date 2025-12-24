namespace Contract;

public class TokenExpiredException : DomainException
{
    public TokenExpiredException(string message) : base("Unauthorized", message)
    {
    }
}
