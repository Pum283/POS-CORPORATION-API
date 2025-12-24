namespace Contract;
public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base("Not found" ,message)
    {
    }
}
