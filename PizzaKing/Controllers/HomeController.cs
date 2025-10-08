using Microsoft.AspNetCore.Mvc;
using PizzaKing.Interfaces;
using PizzaKing.Models;
using PizzaKing.Models.Pages;
using PizzaKing.ViewModels;
using System.Diagnostics;

namespace PizzaKing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProduct _products;
        private readonly ICategory _categories;
        private readonly IReview _reviews;

        public HomeController(IProduct products, ICategory categories, IReview reviews)
        {
            _products = products;
            _categories = categories;
            _reviews = reviews;
        }

        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index(QueryOptions options)
        {
            var latestReviews = await _reviews.GetReviewsAsync(4);
            var reviewsVm = latestReviews
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    Author = r.Author,
                    Text = r.Text,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt.ToLocalTime().ToString("dd MMM yyyy"),
                })
                .ToList();

            ViewBag.Reviews = reviewsVm;
            return View();
        }

        [Route("/menu")]
        [HttpGet]
        public async Task<IActionResult> Menu(QueryOptions options, int categoryId)
        {
            if (categoryId != 0)
            {
                ViewBag.CategoryId = categoryId;

                var currentCategory = await _categories.GetCategoryAsync(categoryId);
                if (currentCategory != null)
                {
                    ViewData["Title"] = currentCategory.Name;
                }
                return View(_products.GetAllProductsByCategory(options, categoryId));
            }
            else
            {
                ViewData["Title"] = "Главная";
                return View(_products.GetAllProducts(options));
            }
        }

        [Route("/product")]
        [HttpGet]
        public async Task<IActionResult> GetProduct(int productId, string? returnUrl)
        {
            if (productId > 0)
            {
                var currentProduct = await _products.GetProductWithCategoryAsync(productId);
                if (currentProduct != null)
                {
                    //Что бы не была установлена активной главная страница.
                    ViewBag.CategoryId = double.NaN;
                    return View(new CurrentProductViewModel
                    {
                        Product = currentProduct,
                        ReturnUrl = returnUrl
                    });
                }
            }
            return NotFound();
        }

    }

}
