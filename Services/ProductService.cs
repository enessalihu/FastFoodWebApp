using FastFoodWeb.Models;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("Products");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductViewModel>>(content, _options);
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Products/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductViewModel>(content, _options);
        }

        public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(string category)
        {
            var response = await _httpClient.GetAsync($"Products/category/{category}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductViewModel>>(content, _options);
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(int productId, int count = 3)
        {
            var product = await GetProductByIdAsync(productId);
            if (product == null)
            {
                return new List<ProductViewModel>();
            }

            var categoryProducts = await GetProductsByCategoryAsync(product.Category);

            
            return categoryProducts
                .Where(p => p.Id != productId)
                .Take(count)
                .ToList();
        }
        public async Task<List<string>> GetCategoriesAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Select(p => p.Category).Distinct().ToList();
        }
    }
}