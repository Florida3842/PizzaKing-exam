using Microsoft.EntityFrameworkCore;
using PizzaKing.Interfaces;
using PizzaKing.Models;
using PizzaKing.Models.Custom;

namespace PizzaKing.Repositories
{
    public class CartRepository : ICart
    {
        private readonly ApplicationContext _applicationContext;

        public CartRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public string ShopCartId { get; set; }

        public static CartRepository GetCart(IServiceProvider service)
        {
            //объект, с помощью которого, мы сможем работать с сессией
            ISession session = service.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = service.GetService<ApplicationContext>();
            //получаем Id корзины пользователя из сессии, если значения нет, создаем новый идентификатор
            string shopCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            //устанавливаем ID как сессию, если такого ID не было, создается новая сессия
            //если есть, то ничего не меняется
            session.SetString("CartId", shopCartId);
            return new CartRepository(context) { ShopCartId = shopCartId };
        }

        public async Task AddToCartAsync(Product product, int quantity)
        {
            await _applicationContext.ShopCartItems.AddAsync(new ShopCartItem
            {
                ShopCartId = ShopCartId,
                ProductId = product.Id,
                Price = product.Price,
                Count = quantity
            });
            await _applicationContext.SaveChangesAsync();
        }
        public async Task<int> GetShopCartItemsCountAsync()
        {
            return await _applicationContext.ShopCartItems.Where(e => e.ShopCartId == ShopCartId).CountAsync();
        }
        public async Task<IEnumerable<ShopCartItem>> GetShopCartItemsAsync()
        {
            return await _applicationContext.ShopCartItems
                .Where(e => e.ShopCartId == ShopCartId)
                .Include(e => e.Product)
                .Include(e => e.Ingredients)
                    .ThenInclude(si => si.Ingredient)
                .ToListAsync();
        }
        public async Task<ShopCartItem> GetShopCartItemAsync(int shopCartItemId)
        {
            return await _applicationContext.ShopCartItems
                .Include(e => e.Ingredients)
                    .ThenInclude(si => si.Ingredient)
                .FirstOrDefaultAsync(e => e.Id == shopCartItemId);
        }
        public async Task RemoveFromCartAsync(ShopCartItem shopCartItem)
        {
            _applicationContext.ShopCartItems.Remove(shopCartItem);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task UpdateFromCartAsync(ShopCartItem shopCartItem)
        {
            _applicationContext.ShopCartItems.Update(shopCartItem);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task ClearCartAsync()
        {
            await _applicationContext.ShopCartItems.Where(e => e.ShopCartId == this.ShopCartId).ExecuteDeleteAsync();
        }

        public async Task<CustomCartSelection> GetCartItemSelectionAsync(int shopCartItemId)
        {
            var item = await GetShopCartItemAsync(shopCartItemId);
            if (item == null) return new CustomCartSelection();

            var selection = new CustomCartSelection();
            var crust = item.Ingredients.FirstOrDefault(i => i.Type == IngredientType.Crust);
            var sauce = item.Ingredients.FirstOrDefault(i => i.Type == IngredientType.Sauce);
            selection.CrustId = crust?.IngredientId;
            selection.SauceId = sauce?.IngredientId;
            selection.ToppingIds = item.Ingredients
                .Where(i => i.Type == IngredientType.Topping)
                .Select(i => i.IngredientId)
                .ToList();

            return selection;
        }

        public async Task UpdateCartItemIngredientsAsync(int shopCartItemId, CustomCartSelection selection)
        {
            var item = await _applicationContext.ShopCartItems
                .Include(i => i.Ingredients)
                .FirstOrDefaultAsync(i => i.Id == shopCartItemId);

            if (item == null) return;

            if (item.Ingredients.Any())
            {
                _applicationContext.ShopCartItemIngredients.RemoveRange(item.Ingredients);
            }

            if (selection.CrustId.HasValue)
            {
                item.Ingredients.Add(new ShopCartItemIngredient
                {
                    IngredientId = selection.CrustId.Value,
                    Type = IngredientType.Crust
                });
            }

            if (selection.SauceId.HasValue)
            {
                item.Ingredients.Add(new ShopCartItemIngredient
                {
                    IngredientId = selection.SauceId.Value,
                    Type = IngredientType.Sauce
                });
            }

            if (selection.ToppingIds != null && selection.ToppingIds.Any())
            {
                foreach (var tId in selection.ToppingIds)
                {
                    item.Ingredients.Add(new ShopCartItemIngredient
                    {
                        IngredientId = tId,
                        Type = IngredientType.Topping
                    });
                }
            }

            item.Price = await RecalculatePriceFromIngredientsAsync(item.ProductId, item.Ingredients.Select(i => i.IngredientId).ToList());

            _applicationContext.ShopCartItems.Update(item);
            await _applicationContext.SaveChangesAsync();
        }

        private async Task<decimal> RecalculatePriceFromIngredientsAsync(int productId, List<int> ingredientIds)
        {
            decimal total = 0m;

            var product = await _applicationContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null) total += product.Price;

            if (ingredientIds != null && ingredientIds.Any())
            {
                var ingredients = await _applicationContext.Ingredients.Where(i => ingredientIds.Contains(i.Id)).ToListAsync();
                total += ingredients.Sum(i => i.Price);
            }

            return total;
        }
    }

}
