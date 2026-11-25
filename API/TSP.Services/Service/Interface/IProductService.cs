using TSP.Common.Dto.App.Cart;
using TSP.Common.Dto.Db;

namespace TSP.Services.Service.Interface
{
    public interface IProductService
    {
        Task<AppProduct?> GetProductByIdAsync(string orderId);
        Task<List<CartItemValidated>> ValidateProductsAsync(List<CartItemDefinition> items);
    }
}
