using Body4uHUB.Shared.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Body4uHUB.Shared.Infrastructure.Email
{
    internal class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _smtpHost = configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = configuration["EmailSettings:Username"];
            _smtpPassword = configuration["EmailSettings:Password"];
            _fromEmail = configuration["EmailSettings:FromEmail"];
            _fromName = configuration["EmailSettings:FromName"];
        }

        public async Task SendEmailConfirmation(string receiverEmail, string receiverName, string confirmationLink)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

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

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = "Email Confirmation",
                IsBodyHtml = true,
                Body = body
            };

            mailMessage.To.Add(receiverEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
