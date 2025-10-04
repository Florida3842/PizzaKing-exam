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

        public HomeController(IProduct products, ICategory categories)
        {
            _products = products;
            _categories = categories;
        }

        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index(QueryOptions options, int categoryId)
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
                ViewData["Title"] = "�������";
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
                    //��� �� �� ���� ����������� �������� ������� ��������.
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
