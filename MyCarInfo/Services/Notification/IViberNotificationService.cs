namespace MyCarInfo.Services.Notification
{
    using MyCarInfo.Data;
    using MyCarInfo.Models;

    public interface IViberNotificationService
    {
        Task<Result> SendViberNotificationAsync(ApplicationUser user, string message, CancellationToken cancellationToken = default);
    }
}
