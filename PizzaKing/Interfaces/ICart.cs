using PizzaKing.Models;

namespace PizzaKing.Interfaces
{
    public interface ICart
    {
        string ShopCartId { get; set; }
        Task<int> GetShopCartItemsCountAsync();
        Task<IEnumerable<ShopCartItem>> GetShopCartItemsAsync();
        Task<ShopCartItem> GetShopCartItemAsync(int shopCartItemId);
        Task AddToCartAsync(Product product, int quantity);
        Task RemoveFromCartAsync(ShopCartItem shopCartItem);
        Task UpdateFromCartAsync(ShopCartItem shopCartItem);
        Task ClearCartAsync();

        Task UpdateCartItemIngredientsAsync(int shopCartItemId, CustomCartSelection selection);
        Task<CustomCartSelection> GetCartItemSelectionAsync(int shopCartItemId);
    }

}
