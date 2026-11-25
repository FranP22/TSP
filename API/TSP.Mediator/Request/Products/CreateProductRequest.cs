using MediatR;
using TSP.Mediator.Dto.Request.Products;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Products
{
    public class CreateProductRequest : IRequest<BaseResponse>
    {
        public CreateProductRequestDto Dto { get; set; }

        public CreateProductRequest(CreateProductRequestDto dto)
        {
            Dto = dto;
        }
    }
}
