using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TSP.Common.Dto.App.Cart;
using TSP.Common.Dto.Db;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AppProduct?> GetProductByIdAsync(string orderId)
        {
            return await _db.Products
                .Where(x => x.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CartItemValidated>> ValidateProductsAsync(List<CartItemDefinition> items)
        {
            bool hasDuplicates = items
                .GroupBy(x => x.ProductId)
                .Any(g => g.Count() > 1);

            if (hasDuplicates) throw new ValidationException("Duplicate products are not allowed.");

            var productIds = items
                .Select(x => x.ProductId)
                .ToList();

            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var result = new List<CartItemValidated>();

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ProductId, out var product)) continue;

                result.Add(new CartItemValidated
                {
                    ProductId = item.ProductId,
                    ProductCount = item.ProductCount,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Valid = product.StockQuantity >= item.ProductCount
                });
            }

            return result;
        }
    }
}
