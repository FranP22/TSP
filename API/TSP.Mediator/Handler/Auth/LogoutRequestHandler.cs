using MediatR;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Request.Auth;

namespace TSP.Mediator.Handler.Auth
{
    public class LogoutRequestHandler : IRequestHandler<LogoutRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            var token = request.RefreshToken;

            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException("Invalid or missing refresh token.");

            return new BaseResponse();
        }
    }
}
