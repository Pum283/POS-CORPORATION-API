namespace Contract;

public class ForbiddenException : DomainException
{
    public ForbiddenException( string message) 
        : base("Forbidden", message)
    {
    }
}
