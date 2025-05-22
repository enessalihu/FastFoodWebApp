using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FastFoodWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(IHttpClientFactory httpClientFactory, ILogger<DiagnosticController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _logger = logger;
        }

        [HttpGet("test-register")]
        public async Task<IActionResult> TestRegister()
        {
            try
            {
                var testUser = new
                {
                    Name = "Test User",
                    Email = $"test{DateTime.Now.Ticks}@example.com",
                    Password = "Test123!",
                    Phone = "1234567890",
                    Address = "Test Address",
                    Role = "customer"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(testUser),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Users", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Test registration response: {responseContent}");

                return Ok(new
                {
                    StatusCode = response.StatusCode,
                    Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value),
                    Content = responseContent,
                    ContentType = response.Content.Headers.ContentType?.ToString(),
                    IsSuccess = response.IsSuccessStatusCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test registration failed");
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpGet("test-users")]
        public async Task<IActionResult> TestGetUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync("Users");
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Test get users response: {responseContent}");

                return Ok(new
                {
                    StatusCode = response.StatusCode,
                    Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value),
                    Content = responseContent,
                    ContentType = response.Content.Headers.ContentType?.ToString(),
                    IsSuccess = response.IsSuccessStatusCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test get users failed");
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}