using MediatR;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;

namespace TSP.Mediator.Request.Orders
{
    public class GetOrderByIdRequest : IRequest<BaseResponse<OrderResposeDto>>
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public GetOrderByIdRequest(string id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}
