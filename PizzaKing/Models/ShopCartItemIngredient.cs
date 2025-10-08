using PizzaKing.Models.Custom;
namespace PizzaKing.Models
{
    public class ShopCartItemIngredient
    {
        public int Id { get; set; }

        public int ShopCartItemId { get; set; }
        public ShopCartItem ShopCartItem { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        public IngredientType Type { get; set; }
    }
}
