namespace MyCarInfo.Services.Notification
{
    using Microsoft.Extensions.Options;
    using MyCarInfo.Models.Options;
    using System.Net;
    using System.Net.Mail;

    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly EmailOptions _options;

        public EmailNotificationService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(_options.Host)
                || string.IsNullOrWhiteSpace(_options.SenderEmail)
                || string.IsNullOrWhiteSpace(_options.Username)
                || string.IsNullOrWhiteSpace(_options.Password))
            {
                throw new InvalidOperationException("Email settings are not configured.");
            }

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_options.SenderEmail, _options.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            message.To.Add(recipientEmail);
            await client.SendMailAsync(message, cancellationToken);
        }
    }
}
