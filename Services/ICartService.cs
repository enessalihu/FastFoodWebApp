using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface ICartService
    {
        Task<List<CartViewModel>> GetCartItemsAsync(int userId);
        Task<CartViewModel> AddToCartAsync(int userId, int productId, int quantity);
        Task UpdateCartItemAsync(int cartItemId, int quantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task ClearCartAsync(int userId);
    }
}