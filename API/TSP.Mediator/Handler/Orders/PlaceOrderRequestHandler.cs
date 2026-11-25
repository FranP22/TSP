using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TSP.Common.Exceptions;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;
using TSP.Mediator.Request.Orders;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Orders
{
    public class PlaceOrderRequestHandler : IRequestHandler<PlaceOrderRequest, BaseResponse>
    {
        private readonly AppDbContext _db;
        private readonly IProductService _product;
        private readonly IOrderService _order;

        public PlaceOrderRequestHandler(AppDbContext db, IProductService product, IOrderService order)
        {
            _db = db;
            _product = product;
            _order = order;
        }

        public async Task<BaseResponse> Handle(PlaceOrderRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(x => x.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("User not found.");

            var validated = await _product.ValidateProductsAsync(request.Dto.Items);

            var outOfStock = validated
                .Where(x => !x.Valid)
                .Select(x => x.ProductName)
                .ToList();

            if (outOfStock.Any())
            {
                var message = $"Following products are out of stock: {string.Join(", ", outOfStock)}";

                return new BaseResponse
                {
                    Success = false,
                    Message = message
                };
            }

            var order = await _order.PlaceOrderAsync(request.UserId, validated);

            if (order == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Failed to place an order."
                };
            }

            return new BaseResponse
            {
                Success = true,
                Message = "Successfully placed an order."
            };
        }
    }
}
