using FastFoodWeb.Models;

namespace FastFoodWeb.Services
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetOrdersAsync();
        Task<OrderViewModel> GetOrderByIdAsync(int id);
        Task<List<OrderViewModel>> GetOrdersByUserAsync(int userId);
        Task<OrderViewModel> CreateOrderAsync(int userId, string deliveryAddress, List<OrderItemViewModel> items);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}