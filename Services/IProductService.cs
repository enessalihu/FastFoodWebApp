using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface IProductService
    {
        Task<List<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task<List<ProductViewModel>> GetProductsByCategoryAsync(string category);
        Task<List<string>> GetCategoriesAsync();
        Task<List<ProductViewModel>> GetRelatedProductsAsync(int productId, int count = 3);
    }
}