namespace MyCarInfo.Data
{
    public class Notification
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public string DocumentType { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
