using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Body4uHUB.Shared.Infrastructure.Email
{
    internal sealed class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;

        public Task SendEmailConfirmation(string receiverEmail, string firstName, string lastName, string confirmationLink)
        {
            var body = EmailTemplates.Render(EmailTemplates.ConfirmEmail, Values(firstName, lastName, confirmationLink));
            return Send(receiverEmail, "Потвърди имейла си в Body4uHUB", body);
        }

        public Task SendPasswordReset(string receiverEmail, string firstName, string lastName, string resetLink)
        {
            var body = EmailTemplates.Render(EmailTemplates.ResetPassword, Values(firstName, lastName, resetLink));
            return Send(receiverEmail, "Нова парола за Body4uHUB", body);
        }

        private static Dictionary<string, string> Values(string firstName, string lastName, string link) => new()
        {
            ["firstName"] = firstName,
            ["lastName"] = lastName,
            ["link"] = link,
        };

        private async Task Send(string receiverEmail, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Body = body,
                BodyEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(receiverEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
