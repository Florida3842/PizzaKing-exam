using Microsoft.AspNetCore.Mvc;
using PizzaKing.Services;
using System.Threading.Tasks;

namespace PizzaKing.Controllers
{
    public class InfoController : Controller
    {
        private readonly IEmailSender _email;
        public InfoController(IEmailSender email) => _email = email;

        [HttpGet]
        public IActionResult Contact(string? submitted = null)
        {
            ViewBag.Submitted = submitted == "true";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError(string.Empty, "Заполните все поля.");
                return View();
            }

            await _email.SendAsync("email@gmail.com", "Сообщение с контактов PizzaStar",
                $"<p><b>Имя:</b> {name}</p><p><b>Email:</b> {email}</p><p>{message}</p>");

            return RedirectToAction(nameof(Contact), new { submitted = "true" });
        }

        [HttpGet]
        public IActionResult Delivery() => View();
    }
}
