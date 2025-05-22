using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface IReviewService
    {
        Task<List<ReviewViewModel>> GetAllReviewsAsync();
        Task<ReviewViewModel> GetReviewByIdAsync(int id);
        Task<List<ReviewViewModel>> GetReviewsByProductAsync(int productId);
        Task<ReviewViewModel> CreateReviewAsync(CreateReviewViewModel model);
        Task UpdateReviewAsync(int id, UpdateReviewViewModel model);
        Task DeleteReviewAsync(int id);
    }
}