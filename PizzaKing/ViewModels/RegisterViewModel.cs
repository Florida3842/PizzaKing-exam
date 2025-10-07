using System.ComponentModel.DataAnnotations;

namespace PizzaKing.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Укажите год рождения")]
        [Range(1900, 2100, ErrorMessage = "Год вне диапазона")]
        [Display(Name = "Год рождения")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Укажите город")]
        [Display(Name = "Город")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Укажите адрес")]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
