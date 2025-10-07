using System.ComponentModel.DataAnnotations;

namespace PizzaKing.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress, Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
