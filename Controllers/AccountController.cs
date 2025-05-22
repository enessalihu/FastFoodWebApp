using FastFoodWeb.Models;
using FastFoodWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFoodWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation($"Login attempt for email: {model.Email}");

                    var result = await _userService.LoginAsync(model.Email, model.Password);

                    if (result.Success)
                    {
                        _logger.LogInformation($"Login successful for user: {result.Email}, ID: {result.UserId}");

                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                            new Claim(ClaimTypes.Name, result.Name),
                            new Claim(ClaimTypes.Email, result.Email),
                            new Claim(ClaimTypes.Role, result.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        
                        HttpContext.Session.SetInt32("UserId", result.UserId.Value);

                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    _logger.LogWarning($"Login failed: {result.Message}");
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during login");
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation($"Registration attempt for email: {model.Email}");

                    var result = await _userService.RegisterAsync(model);

                    if (result.Success)
                    {
                        _logger.LogInformation($"Registration successful for user: {result.Email}, ID: {result.UserId}");

                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                            new Claim(ClaimTypes.Name, result.Name),
                            new Claim(ClaimTypes.Email, result.Email),
                            new Claim(ClaimTypes.Role, result.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        
                        HttpContext.Session.SetInt32("UserId", result.UserId.Value);

                        return RedirectToAction("Index", "Home");
                    }

                    _logger.LogWarning($"Registration failed: {result.Message}");
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration");
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}