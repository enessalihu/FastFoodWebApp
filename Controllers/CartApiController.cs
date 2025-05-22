using FastFoodWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastFoodWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartApiController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount(int userId)
        {
            if (userId <= 0)
            {
                return Ok(0);
            }
            
            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                return Ok(cartItems.Count);
            }
            catch
            {
                return Ok(0);
            }
        }
    }
}