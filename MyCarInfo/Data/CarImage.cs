namespace MyCarInfo.Data
{
    public class CarImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
