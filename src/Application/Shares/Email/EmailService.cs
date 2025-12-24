using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application;

public class EmailService(IOptions<EmailSettings> mailSettings) : IEmailService
{
    private readonly EmailSettings _mailSettings = mailSettings.Value;

    public async Task<bool> SendMailAsync(EmailContent mailContent)
    {
        var email = new MimeMessage
        {
            Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail)
        };
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

        email.To.Add(new MailboxAddress(mailContent.To, mailContent.To));
        email.Subject = mailContent.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = mailContent.Body
        };

        email.Body = builder.ToMessageBody();
        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
        }
        catch
        {
            return false;
        }

        smtp.Disconnect(true);
        return true;
    }
}
