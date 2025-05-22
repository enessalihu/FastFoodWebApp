using FastFoodWeb.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class ReviewService : IReviewService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ReviewService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<ReviewViewModel>> GetAllReviewsAsync()
        {
            var response = await _httpClient.GetAsync("Reviews");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ReviewViewModel>>(content, _options);
        }

        public async Task<ReviewViewModel> GetReviewByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Reviews/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReviewViewModel>(content, _options);
        }

        public async Task<List<ReviewViewModel>> GetReviewsByProductAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"Reviews/product/{productId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ReviewViewModel>>(content, _options);
        }

        public async Task<ReviewViewModel> CreateReviewAsync(CreateReviewViewModel model)
        {
            var createReviewDto = new
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            var response = await _httpClient.PostAsJsonAsync("Reviews", createReviewDto);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReviewViewModel>(content, _options);
        }

        public async Task UpdateReviewAsync(int id, UpdateReviewViewModel model)
        {
            var updateReviewDto = new
            {
                Rating = model.Rating,
                Comment = model.Comment
            };

            var response = await _httpClient.PutAsJsonAsync($"Reviews/{id}", updateReviewDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReviewAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Reviews/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}