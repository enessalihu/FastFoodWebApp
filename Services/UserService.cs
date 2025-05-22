using FastFoodWeb.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace FastFoodWeb.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        public async Task<UserViewModel> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Users/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"GetUserByIdAsync response: {content}");
            
            return JsonSerializer.Deserialize<UserViewModel>(content, _options);
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("Users");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserViewModel>>(content, _options);
        }

        public async Task<UserAuthResponseViewModel> LoginAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation($"Attempting login for email: {email}");
               
                
                var response = await _httpClient.GetAsync("Users");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get users: {response.StatusCode}");
                    return new UserAuthResponseViewModel
                    {
                        Success = false,
                        Message = $"Login failed: Unable to retrieve users. Status code: {response.StatusCode}"
                    };
                }
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Users API response: {content}");
                
                var users = JsonSerializer.Deserialize<List<UserViewModel>>(content, _options);
                
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found or deserialization failed");
                    return new UserAuthResponseViewModel
                    {
                        Success = false,
                        Message = "Login failed: No users found"
                    };
                }
                
                
                var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                
                if (user == null)
                {
                    _logger.LogWarning($"User not found for email: {email}");
                    return new UserAuthResponseViewModel
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }
                
                _logger.LogInformation($"User found: {user.Id}, {user.Name}, {user.Email}");
                
               
                string hashedPassword = HashPassword(password);
                _logger.LogInformation($"Hashed password: {hashedPassword}");
                
               
                
                return new UserAuthResponseViewModel
                {
                    Success = true,
                    Message = "Login successful",
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return new UserAuthResponseViewModel
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<UserAuthResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                _logger.LogInformation($"Registering user: {model.Email}");
                
                var createUserDto = new
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Phone = model.Phone,
                    Address = model.Address,
                    Role = "customer"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(createUserDto),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation($"Registration request body: {await content.ReadAsStringAsync()}");
                
                var response = await _httpClient.PostAsync("Users", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Registration API response: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    
                    try
                    {
                        using (JsonDocument document = JsonDocument.Parse(responseContent))
                        {
                            JsonElement root = document.RootElement;
                            
                           
                            int userId = 0;
                            string name = "";
                            string email = "";
                            string role = "customer";
                            
                            
                            if (root.TryGetProperty("id", out JsonElement idElement) || 
                                root.TryGetProperty("Id", out idElement) ||
                                root.TryGetProperty("ID", out idElement))
                            {
                                userId = idElement.GetInt32();
                            }
                            else
                            {
                                _logger.LogWarning("Could not find 'id' property in response");
                               
                                _logger.LogWarning($"Response properties: {string.Join(", ", root.EnumerateObject().Select(p => p.Name))}");
                                
                                
                                var usersResponse = await _httpClient.GetAsync("Users");
                                if (usersResponse.IsSuccessStatusCode)
                                {
                                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                                    var users = JsonSerializer.Deserialize<List<UserViewModel>>(usersContent, _options);
                                    var newUser = users?.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
                                    
                                    if (newUser != null)
                                    {
                                        userId = newUser.Id;
                                        name = newUser.Name;
                                        email = newUser.Email;
                                        role = newUser.Role;
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Could not find newly registered user");
                                    }
                                }
                            }
                            
                         
                            if (root.TryGetProperty("name", out JsonElement nameElement) || 
                                root.TryGetProperty("Name", out nameElement))
                            {
                                name = nameElement.GetString();
                            }
                            else
                            {
                                name = model.Name; 
                            }
                            
                            if (root.TryGetProperty("email", out JsonElement emailElement) || 
                                root.TryGetProperty("Email", out emailElement))
                            {
                                email = emailElement.GetString();
                            }
                            else
                            {
                                email = model.Email; 
                            }
                            
                            if (root.TryGetProperty("role", out JsonElement roleElement) || 
                                root.TryGetProperty("Role", out roleElement))
                            {
                                role = roleElement.GetString();
                            }
                            
                            if (userId > 0)
                            {
                                _logger.LogInformation($"Registration successful: User ID: {userId}, Name: {name}, Email: {email}, Role: {role}");
                                
                                return new UserAuthResponseViewModel
                                {
                                    Success = true,
                                    Message = "Registration successful",
                                    UserId = userId,
                                    Name = name,
                                    Email = email,
                                    Role = role
                                };
                            }
                            else
                            {
                                _logger.LogWarning("Could not extract user ID from response");
                                return new UserAuthResponseViewModel
                                {
                                    Success = false,
                                    Message = "Registration failed: Could not extract user ID from response"
                                };
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse registration response");
                        return new UserAuthResponseViewModel
                        {
                            Success = false,
                            Message = $"Registration failed: Invalid response format - {ex.Message}"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning($"Registration failed: {responseContent}");
                    return new UserAuthResponseViewModel
                    {
                        Success = false,
                        Message = $"Registration failed: {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return new UserAuthResponseViewModel
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}