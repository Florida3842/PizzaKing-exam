using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PizzaKing.Interfaces;
using PizzaKing.Models;
using PizzaKing.Models.Checkout;
using PizzaKing.Models.Pages;
using PizzaKing.Repositories;
using PizzaKing.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace PizzaKing.Controllers
{

    public class OrderController : Controller
    {
        private readonly CartRepository _cartRepository;
        private readonly IOrder _orders;
        private readonly UserManager<User> _userManager;

        public OrderController(CartRepository cartRepository, IOrder orders, UserManager<User> userManager)
        {
            _cartRepository = cartRepository;
            _orders = orders;
            _userManager = userManager;
        }

        [Route("order-info")]
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return NotFound();

                var currentUser = await _userManager.FindByIdAsync(userId);
                return View("Authenticated", new OrderViewModelAuthenticated
                {
                    City = currentUser.City,
                    Address = currentUser.Address
                });
            }
            return View("NonAuthenticated");
        }

        [Route("order-finish-result")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FinishOrderNonAuthenticated(OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid) return View("NonAuthenticated", orderViewModel);

            var items = await _cartRepository.GetShopCartItemsAsync();
            if (!items.Any()) return RedirectToAction("Index", "ShopCart");

            var order = new Order
            {
                Fio = orderViewModel.Fio,
                Email = orderViewModel.Email,
                Phone = orderViewModel.Phone,
                City = orderViewModel.City,
                Address = orderViewModel.Address,
                CreatedAt = DateTime.Now
            };

            order.OrderDetails = items.Select(e => new OrderDetails
            {
                Order = order,
                ProductId = e.ProductId,
                Quantity = e.Count
            }).ToList();

            await _orders.AddOrderAsync(order);

            var amount = items.Sum(i => i.Price * i.Count);
            await _cartRepository.ClearCartAsync();

            return RedirectToAction("Start", "Payment", new { orderId = order.Id, amount });
        }

        [Route("order-finish")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FinishOrder(OrderViewModelAuthenticated orderViewModel)
        {
            if (!ModelState.IsValid) return View("Authenticated", orderViewModel);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var currentUser = await _userManager.FindByIdAsync(userId);

            var order = new Order
            {
                UserId = userId,
                City = orderViewModel.City,
                Address = orderViewModel.Address,
                CreatedAt = DateTime.Now
            };

            if (!string.Equals(currentUser.City, orderViewModel.City, StringComparison.OrdinalIgnoreCase))
            {
                currentUser.City = orderViewModel.City;
                await _userManager.UpdateAsync(currentUser);
            }
            if (!string.Equals(currentUser.Address, orderViewModel.Address, StringComparison.OrdinalIgnoreCase))
            {
                currentUser.Address = orderViewModel.Address;
                await _userManager.UpdateAsync(currentUser);
            }

            var items = await _cartRepository.GetShopCartItemsAsync();
            if (!items.Any()) return RedirectToAction("Index", "ShopCart");

            order.OrderDetails = items.Select(e => new OrderDetails
            {
                Order = order,
                ProductId = e.ProductId,
                Quantity = e.Count
            }).ToList();

            await _orders.AddOrderAsync(order);

            var amount = items.Sum(i => i.Price * i.Count);
            await _cartRepository.ClearCartAsync();

            return RedirectToAction("Start", "Payment", new { orderId = order.Id, amount });
        }

        [Route("orders")]
        [HttpGet]
        public IActionResult MyOrders(QueryOptions options)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();
            return View(_orders.GetAllOrdersByUserWithDetails(options, userId));
        }

        [Authorize(Roles = "Admin")]
        [Route("/panel-orders")]
        [HttpGet]
        public IActionResult Orders(QueryOptions options)
        {
            return View(_orders.GetAllOrdersWithDetails(options));
        }

        [Authorize(Roles = "Admin")]
        [Route("/panel/delete-order")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var currentOrder = await _orders.GetOrderAsync(orderId);
            if (currentOrder != null)
            {
                await _orders.RemoveOrderAsync(currentOrder);
            }
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("/panel/update-order-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(
        int orderId,
        OrderStatus status,
        [FromServices] IOrder orders,
        [FromServices] IHubContext<OrderHub> hub) 
            {
            var order = await orders.GetOrderAsync(orderId);
            if (order is null) return NotFound();

            order.Status = status;
            await orders.EditOrderAsync(order);


            if (!string.IsNullOrEmpty(order.UserId))
            {
                await hub.Clients.Group($"user:{order.UserId}")
                    .SendAsync("orderStatusChanged", new { orderId = order.Id, status = order.Status.ToString() });
            }

            return Ok();
        }

    }
}
