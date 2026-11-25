using MediatR;
using TSP.Mediator.Dto.Request.Orders;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Orders
{
    public class PlaceOrderRequest : IRequest<BaseResponse>
    {
        public PlaceOrderRequestDto Dto { get; set; }
        public string UserId { get; set; }

        public PlaceOrderRequest(PlaceOrderRequestDto dto, string userId)
        {
            Dto = dto;
            UserId = userId;
        }
    }
}
