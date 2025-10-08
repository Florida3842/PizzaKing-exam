using Microsoft.EntityFrameworkCore;
using PizzaKing.Interfaces;
using PizzaKing.Models.Custom;
namespace PizzaKing.Repositories
{
    public class IngredientRepository : IIngredient
    {
        private readonly ApplicationContext _applicationContext;

        public IngredientRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _applicationContext.Ingredients.ToListAsync();
        }

        public async Task<IEnumerable<Ingredient>> GetByTypeAsync(IngredientType type)
        {
            return await _applicationContext.Ingredients
                .Where(i => i.Type == type)
                .ToListAsync();
        }

        public async Task<Ingredient> GetByIdAsync(int id)
        {
            return await _applicationContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
