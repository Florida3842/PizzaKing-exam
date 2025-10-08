using PizzaKing.Models;

namespace PizzaKing.Interfaces
{
    public interface IReview
    {
        Task<IEnumerable<Review>> GetReviewsAsync(int count = 3);
    }
}
