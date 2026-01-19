using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Models
{
    public class ProfileUpdateModel
    {
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невалиден телефонен номер.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилията е задължителна.")]
        public string LastName { get; set; } = string.Empty;
    }
}
