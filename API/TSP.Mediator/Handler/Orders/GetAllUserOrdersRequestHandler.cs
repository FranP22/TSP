using MediatR;
using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Filter;
using TSP.Common.Exceptions;
using TSP.Common.Extensions;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;
using TSP.Mediator.Request.Orders;

namespace TSP.Mediator.Handler.Orders
{
    public class GetAllUserOrdersRequestHandler : 
        IRequestHandler<
        GetAllUserOrdersRequest, 
        BaseResponse<PaginationResponse<OrderResposeDto>>>
    {
        private readonly AppDbContext _db;

        public GetAllUserOrdersRequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<BaseResponse<PaginationResponse<OrderResposeDto>>> Handle(GetAllUserOrdersRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                .Where(x => x.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("User not found.");

            var response = await _db.Orders
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .Select(x => new OrderResposeDto()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Total = x.Total,
                    CreatedAt = x.CreatedAt
                })
                .AsQueryable()
                .PaginateAsync(request.Pagination);

            return new BaseResponse<PaginationResponse<OrderResposeDto>>
            {
                Value = response,
                Success = true,
                Message = ""
            };
        }
    }
}
