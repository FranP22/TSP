using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.App.Cart;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Filter;
using TSP.Common.Extensions;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IProductService _product;

        public OrderService(AppDbContext db, IProductService product)
        {
            _db = db;
            _product = product;
        }

        public async Task<AppOrder?> GetOrderByIdAsync(string orderId)
        {
            return await _db.Orders
                .Where(x => x.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<AppOrder>> GetUserOrdersAsync(string userId)
        {
            return await _db.Orders
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
        }

        public async Task<AppOrder?> PlaceOrderAsync(string userId, List<CartItemValidated> items)
        {
            if (items == null || items.Count == 0)
                return null;

            if (items.Any(x => !x.Valid || x.ProductCount <= 0))
                return null;

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var description = string.Join(", ", items.Select(x =>
                    $"{x.ProductCount}x {x.ProductName}"
                ));

                var total = items.Sum(x => x.ProductCount * x.ProductPrice);

                var order = new AppOrder
                {
                    UserId = userId,
                    Description = description,
                    Total = total,
                    //Status = OrderStatus.PendingPayment,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Orders.Add(order);

                foreach (var item in items)
                {
                    var product = await _db.Products
                        .FirstOrDefaultAsync(x => x.Id == item.ProductId);

                    if (product == null || product.StockQuantity < item.ProductCount)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    product.StockQuantity -= item.ProductCount;
                }

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
