namespace MyCarInfo.Data
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Engine { get; set; } = string.Empty;
        public int HorsePower { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime InsuranceExpiryDate { get; set; }
        public DateTime InspectionExpiryDate { get; set; }
        public DateTime VignetteExpiryDate { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
