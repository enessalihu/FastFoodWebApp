using FastFoodWeb.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<OrderViewModel>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync("Orders");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderViewModel>>(content, _options);
        }

        public async Task<OrderViewModel> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderViewModel>(content, _options);
        }

        public async Task<List<OrderViewModel>> GetOrdersByUserAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Orders/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderViewModel>>(content, _options);
        }

        public async Task<OrderViewModel> CreateOrderAsync(int userId, string deliveryAddress, List<OrderItemViewModel> items)
        {
            var createOrderDto = new
            {
                UserId = userId,
                DeliveryAddress = deliveryAddress,
                OrderItems = items.Select(i => new { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("Orders", createOrderDto);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderViewModel>(content, _options);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var updateOrderDto = new
            {
                Status = status
            };

            var response = await _httpClient.PutAsJsonAsync($"Orders/{orderId}", updateOrderDto);
            response.EnsureSuccessStatusCode();
        }
    }
}