using PizzaKing.Models.Custom;

namespace PizzaKing.Interfaces
{
    public interface IIngredient
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<IEnumerable<Ingredient>> GetByTypeAsync(IngredientType type);
        Task<Ingredient> GetByIdAsync(int id);
    }
}
