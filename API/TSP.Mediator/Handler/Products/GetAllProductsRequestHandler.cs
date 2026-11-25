using MediatR;
using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Filter;
using TSP.Common.Exceptions;
using TSP.Common.Extensions;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;
using TSP.Mediator.Dto.Response.Products;
using TSP.Mediator.Request.Products;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Products
{
    public class GetAllProductsRequestHandler : IRequestHandler<GetAllProductsRequest, BaseResponse<PaginationResponse<ProductResponseDto>>>
    {
        private readonly AppDbContext _db;
        private readonly IRoleService _role;

        public GetAllProductsRequestHandler(AppDbContext db, IRoleService role)
        {
            _db = db;
            _role = role;
        }

        public async Task<BaseResponse<PaginationResponse<ProductResponseDto>>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(x => x.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("User not found.");

            var canSeeInactive = await _role.HasPermissionAsync(user.RoleId, Common.Enum.RolePermission.ProductGetInactive);

            var response = await _db.Products
                .AsNoTracking()
                .Where(x => !request.ShowOnlyInStock || x.StockQuantity > 0)
                .Where(x => x.IsActive || canSeeInactive)
                .Select(x => new ProductResponseDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    IsActive = x.IsActive,
                    InStock = x.StockQuantity == 0,
                    StockMessage = x.StockQuantity == 0
                        ? "Out of stock"
                        : x.StockQuantity <= 5
                            ? $"Only {x.StockQuantity} left in stock"
                            : "In stock"
                })
                .AsQueryable()
                .PaginateAsync(request.Pagination);

            return new BaseResponse<PaginationResponse<ProductResponseDto>>
            {
                Value = response,
                Success = true,
                Message = ""
            };
        }
    }
}
