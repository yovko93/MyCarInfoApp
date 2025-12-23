namespace MyCarInfo.Services.Notification
{
    using Microsoft.Extensions.Options;
    using MyCarInfo.Data;
    using MyCarInfo.Models;
    using MyCarInfo.Models.Options;

    public class ViberNotificationService : IViberNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViberNotificationService> _logger;
        private readonly ViberOptions _viberOptions;

        public ViberNotificationService(
            HttpClient httpClient,
            IOptions<ViberOptions> options,
            ILogger<ViberNotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _viberOptions = options.Value;
        }

        public async Task<Result> SendViberNotificationAsync(ApplicationUser user, string message, CancellationToken cancellationToken = default)
        {
            var result = new Result()
            {
                Succeeded = false,
                Error = ""
            };

            if (user is null)
            {
                _logger.LogError("Missing user information for Viber notification.");
                result.Error = "Missing user information for Viber notification.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                _logger.LogError("Cannot send Viber notification because user {UserId} does not have a phone number.", user.Id);
                result.Error = $"Cannot send Viber notification because user {user.Id} does not have a phone number.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(_viberOptions.VIBER_BOT_TOKEN))
            {
                _logger.LogError("Viber API token is not configured.");
                result.Error = "Viber API token is not configured.";
                return result;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, _viberOptions.ApiUrl);
            request.Headers.Add("X-Viber-Auth-Token", _viberOptions.VIBER_BOT_TOKEN);

            var payload = new
            {
                receiver = user.PhoneNumber,
                min_api_version = 1,
                type = "text",
                text = message,
                sender = new
                {
                    name = _viberOptions.SenderName
                }
            };

            request.Content = JsonContent.Create(payload);

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    result.Succeeded = true;
                    return result;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Viber notification failed for user {UserId}. StatusCode: {StatusCode}, Response: {Response}",
                    user.Id,
                    response.StatusCode,
                    responseBody);
                result.Error = $"Viber notification failed for user {user.Id}. StatusCode: {response.StatusCode}, Response: {responseBody}";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Viber notification failed for user {UserId}.", user.Id);
                result.Error = $"Viber notification failed for user {user.Id}.";
                return result;
            }
        }
    }
}
