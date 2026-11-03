using MediatR;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Auth
{
    public class LogoutRequest : IRequest<BaseResponse>
    {
        public string? RefreshToken { get; set; }

        public LogoutRequest(string? refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
