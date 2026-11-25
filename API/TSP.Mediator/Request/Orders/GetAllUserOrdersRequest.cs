using MediatR;
using TSP.Common.Dto.Filter;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;

namespace TSP.Mediator.Request.Orders
{
    public class GetAllUserOrdersRequest : IRequest<BaseResponse<PaginationResponse<OrderResposeDto>>>
    {
        public string UserId { get; set; }
        public PaginationRequest Pagination { get; set; }

        public GetAllUserOrdersRequest(string userId, PaginationRequest pagination)
        {
            UserId = userId;
            Pagination = pagination;
        }
    }
}
