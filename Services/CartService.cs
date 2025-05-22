using FastFoodWeb.Models;
using System.Text;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public CartService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CartViewModel>> GetCartItemsAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Cart/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CartViewModel>>(content, _options);
        }

        public async Task<CartViewModel> AddToCartAsync(int userId, int productId, int quantity)
        {
            var createCartDto = new
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createCartDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("Cart", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CartViewModel>(responseContent, _options);
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var updateCartDto = new
            {
                Quantity = quantity
            };

            var content = new StringContent(
                JsonSerializer.Serialize(updateCartDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"Cart/{cartItemId}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var response = await _httpClient.DeleteAsync($"Cart/{cartItemId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ClearCartAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"Cart/user/{userId}");
            response.EnsureSuccessStatusCode();
        }
    }
}