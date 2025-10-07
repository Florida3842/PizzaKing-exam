using System.ComponentModel.DataAnnotations;

namespace PizzaKing.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string? ConfirmPassword { get; set; }
    }
}
