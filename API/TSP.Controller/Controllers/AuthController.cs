using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSP.Controller.Extensions;
using TSP.Mediator.Dto.Request.Auth;
using TSP.Mediator.Request.Auth;

namespace TSP.Controller.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _sender.Send(new LoginRequest(request));

            return Ok(response);
        }
    }
}
