using MediatR;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Request.Auth;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Auth
{
    public class LogoutRequestHandler : IRequestHandler<LogoutRequest, BaseResponse>
    {
        private readonly IAuthService _auth;

        public LogoutRequestHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public async Task<BaseResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            var token = request.RefreshToken;

            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException("Invalid or missing refresh token.");

            await _auth.LogoutAsync(token);

            return new BaseResponse()
            {
                Success = true,
                Message = "Successfuly logged out"
            };
        }
    }
}
