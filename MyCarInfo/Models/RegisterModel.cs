using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефонният номер е задължителен.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Името е задължително.")]
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
