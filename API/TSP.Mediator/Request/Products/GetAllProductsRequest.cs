using MediatR;
using TSP.Common.Dto.Filter;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Dto.Response.Orders;
using TSP.Mediator.Dto.Response.Products;

namespace TSP.Mediator.Request.Products
{
    public class GetAllProductsRequest : IRequest<BaseResponse<PaginationResponse<ProductResponseDto>>>
    {
        public string UserId { get; set; }
        public PaginationRequest Pagination { get; set; }
        public bool ShowOnlyInStock { get; set; }

        public GetAllProductsRequest(string userId, PaginationRequest pagination, bool showOnlyInStock = false)
        {
            UserId = userId;
            Pagination = pagination;
            ShowOnlyInStock = showOnlyInStock;
        }
    }
}
