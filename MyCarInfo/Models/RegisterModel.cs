using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефонният номер е задължителен.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
