using Microsoft.EntityFrameworkCore;
using PizzaKing.Interfaces;
using PizzaKing.Models;
namespace PizzaKing.Repositories
{
    public class ReviewRepository : IReview
    {
        private readonly ApplicationContext _context;
        public ReviewRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Review>> GetReviewsAsync(int count = 3)
        {
            return await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
