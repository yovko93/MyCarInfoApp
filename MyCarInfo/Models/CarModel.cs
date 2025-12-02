using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Models
{
    public class CarModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Марка е задължително поле.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Модел е задължително поле.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Регистрационният номер е задължителен.")]
        public string LicensePlate { get; set; }

        public string Engine { get; set; } = string.Empty;

        public int HorsePower { get; set; }

        public string Color { get; set; } = string.Empty;

        public DateTime InsuranceExpiryDate { get; set; }

        public DateTime InspectionExpiryDate { get; set; }

        public DateTime VignetteExpiryDate { get; set; }
    }
}
