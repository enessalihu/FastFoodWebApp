using FastFoodWeb.Models;
using FastFoodWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodWeb.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order" });
            }

            var orders = await _orderService.GetOrdersByUserAsync(userId.Value);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Order/Details/{id}" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            
            
            if (order.UserId != userId.Value)
            {
                return Forbid();
            }
            
            return View(order);
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order/Checkout" });
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string deliveryAddress)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Create order items from cart items
            var orderItems = cartItems.Select(c => new OrderItemViewModel
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.ProductPrice
            }).ToList();

            // Create and place the order
            var order = await _orderService.CreateOrderAsync(userId.Value, deliveryAddress, orderItems);

            // Clear the cart after successful order
            await _cartService.ClearCartAsync(userId.Value);

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            
            // Ensure the order belongs to the current user
            if (order.UserId != userId.Value)
            {
                return Forbid();
            }
            
            return View(order);
        }
    }
}