using KulupYonetimi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace KulupYonetimi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            if (_settings.TrustServerCertificate)
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            }

            var socketOption = _settings.EnableSsl ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.Auto;
            await client.ConnectAsync(_settings.MailServer, _settings.MailPort, socketOption);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
