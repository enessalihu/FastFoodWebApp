using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserByIdAsync(int id);
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<UserAuthResponseViewModel> LoginAsync(string email, string password);
        Task<UserAuthResponseViewModel> RegisterAsync(RegisterViewModel model);
    }
}