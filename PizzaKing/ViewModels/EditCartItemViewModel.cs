using PizzaKing.Models.Custom;

namespace PizzaKing.ViewModels
{
    public class EditCartItemViewModel
    {
        public int ShopCartItemId { get; set; }
        public int? SelectedCrustId { get; set; }
        public int? SelectedSauceId { get; set; }
        public List<int> SelectedToppingIds { get; set; } = new();

        public IEnumerable<Ingredient> Crusts { get; set; } = new List<Ingredient>();
        public IEnumerable<Ingredient> Sauces { get; set; } = new List<Ingredient>();
        public IEnumerable<Ingredient> Toppings { get; set; } = new List<Ingredient>();

        public decimal CurrentPrice { get; set; }
    }
}
