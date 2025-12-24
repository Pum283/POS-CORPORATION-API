namespace Application;

public interface IEmailService
{
    Task<bool> SendMailAsync(EmailContent mailContent);
}
