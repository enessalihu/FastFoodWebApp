using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface IRestaurantService
    {
        Task<List<RestaurantViewModel>> GetAllRestaurantsAsync();
        Task<RestaurantViewModel> GetRestaurantByIdAsync(int id);
    }
}