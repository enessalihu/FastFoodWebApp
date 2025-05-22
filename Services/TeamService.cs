using FastFoodWeb.Models;
using System.Text.Json;

namespace FastFoodWeb.Services
{
    public class TeamService : ITeamService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly Random _random = new Random();

        public TeamService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FastFoodAPI");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<TeamViewModel>> GetAllTeamMembersAsync()
        {
            var response = await _httpClient.GetAsync("Team");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TeamViewModel>>(content, _options);
        }

        public async Task<TeamViewModel> GetTeamMemberByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Team/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TeamViewModel>(content, _options);
        }

        public async Task<List<TeamViewModel>> GetRandomTeamMembersAsync(int count)
        {
            
            var allTeamMembers = await GetAllTeamMembersAsync();

            if (allTeamMembers.Count <= count)
            {
                return allTeamMembers;
            }

            
            return allTeamMembers
                .OrderBy(x => _random.Next())
                .Take(count)
                .ToList();
        }
    }
}