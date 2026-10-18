using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Body4uHUB.Shared.Infrastructure.Email
{
    internal sealed class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;

        public Task SendEmailConfirmation(string receiverEmail, string receiverName, string confirmationLink)
        {
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                  <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <p>Здравейте, {receiverName}</p>
                    <p>Вашият акаунт беше създаден успешно.</p>
                    <p>Моля потвърдете вашия имейл адрес, като кликнете на бутона по-долу:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                      <a href='{confirmationLink}'
                         style='display: inline-block; padding: 15px 30px; background-color: #27ae60; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                         Потвърди имейл
                      </a>
                    </div>
                  </div>
                </body>
                </html>";

            return Send(receiverEmail, "Email Confirmation", body);
        }

        // Interim template in the confirmation email's style; the branded version comes with the design.
        public Task SendPasswordReset(string receiverEmail, string receiverName, string resetLink)
        {
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                  <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <p>Здравейте, {receiverName}</p>
                    <p>Получихме заявка за смяна на паролата на вашия акаунт.</p>
                    <p>Кликнете на бутона по-долу, за да зададете нова парола. Линкът е валиден 1 час и може да се използва само веднъж.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                      <a href='{resetLink}'
                         style='display: inline-block; padding: 15px 30px; background-color: #27ae60; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                         Задай нова парола
                      </a>
                    </div>
                    <p>Ако не сте поискали смяна на паролата, игнорирайте този имейл — паролата ви остава същата.</p>
                  </div>
                </body>
                </html>";

            return Send(receiverEmail, "Възстановяване на парола", body);
        }

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
                IsBodyHtml = true,
                Body = body
            };

            mailMessage.To.Add(receiverEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
