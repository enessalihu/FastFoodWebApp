using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface ITeamService
    {
        Task<List<TeamViewModel>> GetAllTeamMembersAsync();
        Task<TeamViewModel> GetTeamMemberByIdAsync(int id);
        Task<List<TeamViewModel>> GetRandomTeamMembersAsync(int count);
    }
}