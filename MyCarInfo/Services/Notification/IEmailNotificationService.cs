namespace MyCarInfo.Services.Notification
{
    public interface IEmailNotificationService
    {
        Task SendAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken);
    }
}
