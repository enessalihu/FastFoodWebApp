using FastFoodWeb.Models;
using FastFoodWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, IProductService productService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("User ID not found in session");
                    return RedirectToAction("Login", "Account", new { returnUrl = "/Cart" });
                }

                _logger.LogInformation($"Getting cart items for user ID: {userId.Value}");
                var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
                
                // Create a CartSummaryViewModel instead of passing the list directly
                var cartSummary = new CartSummaryViewModel
                {
                    CartItems = cartItems
                };
                
                return View(cartSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(new CartSummaryViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("User ID not found in session");
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.Headers["Referer"].ToString() });
                }

                _logger.LogInformation($"Adding product {productId} to cart for user {userId.Value}");
                await _cartService.AddToCartAsync(userId.Value, productId, quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {productId} to cart");
                TempData["ErrorMessage"] = $"Error adding to cart: {ex.Message}";
                return RedirectToAction("Details", "Products", new { id = productId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                _logger.LogInformation($"Updating quantity for cart item {cartItemId} to {quantity}");
                await _cartService.UpdateCartItemAsync(cartItemId, quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quantity for cart item {cartItemId}");
                TempData["ErrorMessage"] = $"Error updating quantity: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                _logger.LogInformation($"Removing cart item {cartItemId}");
                await _cartService.RemoveFromCartAsync(cartItemId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {cartItemId}");
                TempData["ErrorMessage"] = $"Error removing item: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    _logger.LogInformation($"Clearing cart for user {userId.Value}");
                    await _cartService.ClearCartAsync(userId.Value);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["ErrorMessage"] = $"Error clearing cart: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}