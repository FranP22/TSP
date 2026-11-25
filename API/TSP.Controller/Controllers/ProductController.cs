using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TSP.Common.Dto.Filter;
using TSP.Common.Enum;
using TSP.Controller.Auth;
using TSP.Mediator.Dto.Request.Products;
using TSP.Mediator.Request.Products;

namespace TSP.Controller.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationRequest pagination, [FromQuery] bool showOnlyInStock = false)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var response = await _sender.Send(new GetAllProductsRequest(userId, pagination, showOnlyInStock));

            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById([FromRoute] string id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var response = await _sender.Send(new GetProductByIdRequest(id, userId));

            if (response.Value == null)
            {
                return NotFound(response);
            }

            return Ok();
        }

        [HttpPost]
        [HasPermission(RolePermission.ProductCreate)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            var response = await _sender.Send(new CreateProductRequest(dto));

            return Ok();
        }

        /*[HttpPatch("{id}")]
        [HasPermission(RolePermission.ProductUpdate)]
        public async Task<IActionResult> UpdateProduct()
        {

        }

        [HttpDelete("{id}")]
        [HasPermission(RolePermission.ProductDelete)]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {
            return Ok();
        }*/
    }
}
