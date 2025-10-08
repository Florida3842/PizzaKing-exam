using Microsoft.AspNetCore.Mvc;
using PizzaKing.Interfaces;
using PizzaKing.Models;
using PizzaKing.Models.Custom;
using PizzaKing.Repositories;
using PizzaKing.ViewModels;

namespace PizzaKing.Controllers
{
    public class ShopCartController : Controller
    {
        private readonly IProduct _products;
        private readonly CartRepository _cartRepository;
        private readonly IIngredient _ingredientRepository;

        public ShopCartController(IProduct products, CartRepository cartRepository, IIngredient ingredientRepository)
        {
            _products = products;
            _cartRepository = cartRepository;
            _ingredientRepository = ingredientRepository;
        }

        [Route("ShopCart")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Что бы не была установлена активной главная страница.
            ViewBag.CategoryId = double.NaN;
            var products = await _cartRepository.GetShopCartItemsAsync();
            return View(products);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddToCart(int productId, string? returnUrl, int quantity = 1, bool isModal = true)
        {
            var product = await _products.GetProductAsync(productId);
            if (product != null)
            {
                await _cartRepository.AddToCartAsync(product, quantity);
                if (isModal)
                {
                    return PartialView("_ConfirmModal", (product.Name, quantity));
                }
            }
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> IncrementQuantity(int shopCartItemId)
        {
            var currentShopCartItem = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
            if (currentShopCartItem != null)
            {
                currentShopCartItem.Count += 1;
                await _cartRepository.UpdateFromCartAsync(currentShopCartItem);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DecrementQuantity(int shopCartItemId)
        {
            var currentShopCartItem = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
            if (currentShopCartItem != null)
            {
                if (currentShopCartItem.Count > 1)
                {
                    currentShopCartItem.Count -= 1;
                    await _cartRepository.UpdateFromCartAsync(currentShopCartItem);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RemoveFromCart(int shopCartItemId)
        {
            var currentShopCartItem = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
            if (currentShopCartItem != null)
            {
                await _cartRepository.RemoveFromCartAsync(currentShopCartItem);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditIngredients(int shopCartItemId)
        {
            var cartItem = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
            if (cartItem == null) return NotFound();

            var model = new EditCartItemViewModel
            {
                ShopCartItemId = cartItem.Id,
                SelectedCrustId = cartItem.Ingredients.FirstOrDefault(i => i.Type == IngredientType.Crust)?.IngredientId ?? 0,
                SelectedSauceId = cartItem.Ingredients.FirstOrDefault(i => i.Type == IngredientType.Sauce)?.IngredientId ?? 0,
                SelectedToppingIds = cartItem.Ingredients.Where(i => i.Type == IngredientType.Topping).Select(i => i.IngredientId).ToList(),
                Crusts = await _ingredientRepository.GetByTypeAsync(IngredientType.Crust),
                Sauces = await _ingredientRepository.GetByTypeAsync(IngredientType.Sauce),
                Toppings = await _ingredientRepository.GetByTypeAsync(IngredientType.Topping)
            };

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditIngredients(EditCartItemViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var selection = new CustomCartSelection
            {
                CrustId = model.SelectedCrustId,
                SauceId = model.SelectedSauceId,
                ToppingIds = model.SelectedToppingIds
            };

            await _cartRepository.UpdateCartItemIngredientsAsync(model.ShopCartItemId, selection);

            return RedirectToAction(nameof(Index));
        }
    }

}
