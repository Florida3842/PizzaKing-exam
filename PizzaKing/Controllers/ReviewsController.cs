using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaKing.Models;
using PizzaKing.ViewModels;

namespace PizzaKing.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationContext _context;

        public ReviewsController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Author = r.Author,
                    Text = r.Text,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt.ToLocalTime().ToString("dd MMM yyyy"),
                })
                .ToListAsync();

            return View("~/Views/Review/Index.cshtml", reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var review = new Review
            {
                Author = model.Author,
                Text = model.Text,
                Rating = model.Rating,
                CreatedAt = DateTime.UtcNow
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
