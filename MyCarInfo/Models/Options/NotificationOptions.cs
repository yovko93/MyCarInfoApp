namespace MyCarInfo.Models.Options
{
    public class NotificationOptions
    {
        public const int DefaultCheckIntervalMinutes = 1;
        public const int DefaultCheckIntervalDays = 1;

        public int CheckIntervalMinutes { get; set; } = DefaultCheckIntervalMinutes;
        public int CheckIntervalDays { get; set; } = DefaultCheckIntervalDays;
    }
}
