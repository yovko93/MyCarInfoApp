using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Текущата парола е задължителна.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Новата парола е задължителна.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Новата парола трябва да е поне 6 символа.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потвърждението е задължително.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Новатa парола не съвпада.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
