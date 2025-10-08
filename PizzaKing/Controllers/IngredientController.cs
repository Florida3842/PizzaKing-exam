using Microsoft.AspNetCore.Mvc;
using PizzaKing.Interfaces;
using PizzaKing.Models.Custom;

namespace PizzaKing.Controllers
{
    [Route("ingredients")]
    public class IngredientController : Controller
    {
        private readonly IIngredient _ingredientRepository;

        public IngredientController(IIngredient ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients(CancellationToken cancellationToken = default)
        {
            var ingredients = await _ingredientRepository.GetAllIngredientsAsync();
            return Ok(ingredients);
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredientsByType([FromRoute] IngredientType type, CancellationToken cancellationToken = default)
        {
            var ingredients = await _ingredientRepository.GetByTypeAsync(type);
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredientById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

    } 
}
