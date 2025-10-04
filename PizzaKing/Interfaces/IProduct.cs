using PizzaKing.Models;
using PizzaKing.Models.Pages;

namespace PizzaKing.Interfaces
{
    public interface IProduct
    {
        PagedList<Product> GetAllProducts(QueryOptions options);
        Task<Product> GetProductAsync(int id);
        Task<Product> GetProductWithCategoryAsync(int id);
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task EditProductAsync(Product product);
        PagedList<Product> GetAllProductsByCategory(QueryOptions options, int categoryId);
        Task<IEnumerable<Product>> GetEightRandomProductsAsync(int productId);
    }

}
