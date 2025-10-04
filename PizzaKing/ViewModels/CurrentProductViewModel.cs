using PizzaKing.Models;

namespace PizzaKing.ViewModels
{
    public class CurrentProductViewModel
    {
        public Product Product { get; set; }
        public string? ReturnUrl { get; set; }
    }

}
