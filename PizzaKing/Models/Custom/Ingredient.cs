using System.ComponentModel.DataAnnotations;

namespace PizzaKing.Models.Custom
{
    public class Ingredient
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public IngredientType Type { get; set; }
        public decimal Price { get; set; }
    }
}
