using MediatR;
using Microsoft.EntityFrameworkCore;
using TSP.Common.Enum;
using TSP.Common.Exceptions;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;
using TSP.Mediator.Request.Orders;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Orders
{
    public class GetOrderByIdRequestHandler : IRequestHandler<GetOrderByIdRequest, BaseResponse<OrderResposeDto>>
    {
        private readonly AppDbContext _db;
        private readonly IOrderService _order;
        private readonly IRoleService _role;

        public GetOrderByIdRequestHandler(AppDbContext db, IOrderService order, IRoleService role)
        {
            _db = db;
            _order = order;
            _role = role;
        }

        public async Task<BaseResponse<OrderResposeDto>> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(x => x.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("User not found.");

            var order = await _order.GetOrderByIdAsync(request.Id);

            var hasPermission = await _role.HasPermissionAsync(user.RoleId, RolePermission.OrderGetAny);

            if (order == null || 
                (!hasPermission && user.Id != order?.UserId))
            {
                return new BaseResponse<OrderResposeDto>()
                {
                    Success = false,
                    Message = "Order not found."
                };
            }

            return new BaseResponse<OrderResposeDto>()
            {
                Value = new OrderResposeDto()
                {
                    Description = order.Description,
                    Total = order.Total,
                    CreatedAt = DateTime.UtcNow,
                },
                Success = true,
                Message = ""
            };
        }
    }
}
