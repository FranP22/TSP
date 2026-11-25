using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TSP.Common.Dto.Filter;
using TSP.Common.Enum;
using TSP.Controller.Auth;
using TSP.Controller.Extensions;
using TSP.Mediator.Dto.Request.Auth;
using TSP.Mediator.Dto.Request.Orders;
using TSP.Mediator.Request.Orders;

namespace TSP.Controller.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [HasPermission(RolePermission.OrderGet)]
        public async Task<IActionResult> GetAllUserOrders([FromQuery] PaginationRequest pagination)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var response = await _sender.Send(new GetAllUserOrdersRequest(userId, pagination));

            return Ok(response);
        }

        [HttpGet("{id}")]
        [HasPermission(RolePermission.OrderGet)]
        public async Task<IActionResult> GetOrderById([FromRoute] string id)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var response = await _sender.Send(new GetOrderByIdRequest(id, userId));

            if (response.Value == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [HasPermission(RolePermission.OrderCreate)]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequestDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var response = await _sender.Send(new PlaceOrderRequest(dto, userId));

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
