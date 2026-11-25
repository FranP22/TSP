using MediatR;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Products;

namespace TSP.Mediator.Request.Products
{
    public class GetProductByIdRequest : IRequest<BaseResponse<ProductResponseDto>>
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public GetProductByIdRequest(string id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}
