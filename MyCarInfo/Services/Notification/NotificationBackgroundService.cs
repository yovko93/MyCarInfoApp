namespace MyCarInfo.Services.Notification
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MyCarInfo.Data;
    using MyCarInfo.Models.Options;
    using System.Threading;

    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly NotificationOptions _options;

        public NotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<NotificationOptions> options,
            ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromDays(Math.Max(_options.CheckIntervalDays, NotificationOptions.DefaultCheckIntervalDays));
            _logger.LogInformation("Notification background service started with interval {DelayDays} days.", delay.TotalDays);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking expiring documents for cars...");
                    await CheckExpiringDocumentsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking expiring documents.");
                }

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignore cancellation exceptions when stopping the service
                }
            }

            _logger.LogInformation("Notification background service stopping.");
        }

        private async Task CheckExpiringDocumentsAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow.Date;
            var threshold = now.AddDays(14);

            var vehicles = await context.Cars
                .Where(v => !v.IsDeleted)
                .Include(v => v.User)
                .ToListAsync(cancellationToken);

            foreach (var vehicle in vehicles)
            {
                await EvaluateDocumentAsync(context, vehicle, "Insurance", vehicle.InsuranceExpiryDate, now, threshold, cancellationToken);
                await EvaluateDocumentAsync(context, vehicle, "Inspection", vehicle.InspectionExpiryDate, now, threshold, cancellationToken);
                await EvaluateDocumentAsync(context, vehicle, "Vignette", vehicle.VignetteExpiryDate, now, threshold, cancellationToken);
            }
        }

        private async Task EvaluateDocumentAsync(
            AppDbContext context,
            Vehicle vehicle,
            string documentType,
            DateTime expiryDate,
            DateTime now,
            DateTime threshold,
            CancellationToken cancellationToken)
        {
            try
            {
                if (expiryDate.Date < now || expiryDate.Date > threshold)
                {
                    return;
                }

                var existingNotification = await context.Notifications
                    .AsNoTracking()
                    .AnyAsync(n =>
                    n.VehicleId == vehicle.Id
                    && n.DocumentType == documentType
                    && n.ExpiryDate == expiryDate.Date,
                    cancellationToken);

                if (existingNotification)
                {
                    return;
                }

                var message = BuildMessage(vehicle, documentType, expiryDate);

                var notification = new Notification
                {
                    VehicleId = vehicle.Id,
                    UserId = vehicle.UserId,
                    DocumentType = documentType,
                    ExpiryDate = expiryDate.Date,
                    Channel = "",
                    Message = message,
                    IsSent = false,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Prepared notification for user {UserName} vehicle {VehicleId} ({Brand} {Model}) document {DocumentType} expiring {ExpiryDate:dd.MM.yyyy}.",
                    vehicle.User.UserName,
                    vehicle.Id,
                    vehicle.Brand,
                    vehicle.Model,
                    documentType,
                    expiryDate);

                //var result = await _viberNotificationService.SendViberNotificationAsync(vehicle.User, message, cancellationToken);

                //notification.IsSent = result.Succeeded;
                //notification.SentAt = result.Succeeded ? DateTime.UtcNow : null;
                //notification.ErrorMessage = result.Error;

                //await context.Notifications.AddAsync(notification, cancellationToken);
                //await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        private static string BuildMessage(Vehicle vehicle, string documentType, DateTime expiryDate)
        {
            return $"Документът {documentType} за {vehicle.Brand} {vehicle.Model} ({vehicle.LicensePlate}) изтича на {expiryDate:dd.MM.yyyy}. Моля, подновете навреме.";
        }
    }
}
