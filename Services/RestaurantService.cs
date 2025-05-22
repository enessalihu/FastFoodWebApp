using FastFoodWeb.Models;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public RestaurantService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<RestaurantViewModel>> GetAllRestaurantsAsync()
        {
            var response = await _httpClient.GetAsync("Restaurant");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RestaurantViewModel>>(content, _options);
        }

        public async Task<RestaurantViewModel> GetRestaurantByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Restaurant/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RestaurantViewModel>(content, _options);
        }
    }
}