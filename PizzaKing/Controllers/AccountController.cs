using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PizzaKing.Models;
using PizzaKing.Services;
using PizzaKing.ViewModels;
using System.Text;

namespace PizzaKing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // ===== Регистрация =====
        [AllowAnonymous]
        [HttpGet, Route("register")]
        public IActionResult Register(string? returnUrl = null)
            => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost, Route("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _userManager.FindByEmailAsync(model.Email!);
            if (exists != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Этот email уже зарегистрирован");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.Phone,
                Year = model.Year,
                City = model.City,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
                var confirmLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = tokenEnc }, protocol: Request.Scheme)!;

                await _emailSender.SendAsync(user.Email!, "Подтверждение email",
                    $"<p>Подтвердите почту: <a href=\"{confirmLink}\">{confirmLink}</a></p>");

                await _userManager.AddToRoleAsync(user, "Client");
                await _signInManager.SignInAsync(user, isPersistent: true);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);

            return View(model);
        }

        // ===== Подтверждение e-mail =====
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (user.EmailConfirmed)
            {
                ViewBag.Succeeded = true;
                return View();
            }

            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                ViewBag.Succeeded = false;
                ViewBag.Error = "Некорректная или повреждённая ссылка.";
                return View();
            }

            var result = await _userManager.ConfirmEmailAsync(user, decoded);
            ViewBag.Succeeded = result.Succeeded;
            ViewBag.Error = string.Join("; ", result.Errors.Select(e => e.Description));
            return View();
        }
        //Отправить снова
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Login");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
                return View("ResendConfirmationDone");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = tokenEnc }, protocol: Request.Scheme)!;

            await _emailSender.SendAsync(email, "Подтверждение email",
                $"<p>Ссылка для подтверждения: <a href=\"{link}\">{link}</a></p>");

            return View("ResendConfirmationDone");
        }


        // ===== «Забыли пароль?» =====
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var link = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = tokenEnc }, protocol: Request.Scheme)!;

                await _emailSender.SendAsync(user.Email!, "Сброс пароля",
                    $"<p>Для сброса пароля перейдите по ссылке:</p><p><a href=\"{link}\">{link}</a></p>");
            }

            return View("ForgotPasswordConfirmation");
        }

        // ===== Сброс пароля =====
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest();
            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId!);
            if (user == null) return View("ResetPasswordConfirmation");

            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token!));
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "Недействительный токен.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, decoded, model.Password!);
            if (result.Succeeded) return View("ResetPasswordConfirmation");

            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        // ===== Вход / Выход =====
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email!, model.Password!, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
