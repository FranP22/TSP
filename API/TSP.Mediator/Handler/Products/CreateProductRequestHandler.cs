using MediatR;
using System.ComponentModel.DataAnnotations;
using TSP.Common.Dto.Db;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Request.Products;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Products
{
    public class CreateProductRequestHandler : IRequestHandler<CreateProductRequest, BaseResponse>
    {
        private readonly AppDbContext _db;
        private readonly IProductService _product;

        public CreateProductRequestHandler(AppDbContext db, IProductService product)
        {
            _db = db;
            _product = product;
        }

        public async Task<BaseResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var product = new AppProduct
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            _db.Products.Add(product);

            await _db.SaveChangesAsync();

            return new BaseResponse()
            {
                Success = true,
                Message = "Successfully created a product."
            };
        }
    }
}
