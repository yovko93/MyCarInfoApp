namespace MyCarInfo.Data
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        //public string Color { get; set; }
        public string Engine { get; set; }
        public int HorsePower { get; set; }
        public string LicensePlate { get; set; }
        public DateTime InsuranceExpiryDate { get; set; }
        public DateTime InspectionExpiryDate { get; set; }
        public DateTime VignetteExpiryDate { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
