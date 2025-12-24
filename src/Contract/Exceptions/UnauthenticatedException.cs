namespace Contract;

public class UnauthenticatedException : DomainException
{
    public UnauthenticatedException(string message) 
        : base("Unauthenticated", message)
    {
    }
}
