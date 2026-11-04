using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSP.Common.Enum;
using TSP.Controller.Auth;
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

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            Response.Cookies.SetRefreshTokenCookie(response.Value!.RefreshToken, response.Value.RefreshTokenExpiresAt);

            return Ok(response);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies.GetRefreshTokenCookie();

            var response = await _sender.Send(new LogoutRequest(refreshToken));

            Response.Cookies.DeleteRefreshTokenCookie();

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies.GetRefreshTokenCookie();

            var response = await _sender.Send(new RefreshRequest(refreshToken));

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            Response.Cookies.SetRefreshTokenCookie(response.Value!.RefreshToken, response.Value.RefreshTokenExpiresAt);

            return Ok(response);
        }

        [HttpGet("test-auth")]
        [HasPermission(RolePermission.UserGet)]
        public async Task<IActionResult> Test()
        {
            return Ok("Success");
        }
    }
}
