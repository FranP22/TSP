using TSP.Common.Dto.App.Cart;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Filter;

namespace TSP.Services.Service.Interface
{
    public interface IOrderService
    {
        Task<List<AppOrder>> GetUserOrdersAsync(string userId);
        Task<AppOrder?> GetOrderByIdAsync(string orderId);
        Task<AppOrder?> PlaceOrderAsync(string userId, List<CartItemValidated> items);
    }
}
