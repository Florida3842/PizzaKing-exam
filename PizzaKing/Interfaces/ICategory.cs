using PizzaKing.Models;
using PizzaKing.Models.Pages;

namespace PizzaKing.Interfaces
{
    public interface ICategory
    {
        PagedList<Category> GetAllCategories(QueryOptions options);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
        Task EditCategoryAsync(Category category);
    }
}
