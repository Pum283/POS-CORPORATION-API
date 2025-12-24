namespace Contract;

public class ServiceUnavailableException : DomainException
{
    public ServiceUnavailableException(string messgage) : base("Service Unavailable", messgage)
    {

    }
}
