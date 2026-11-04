using MediatR;
using TSP.Common.Dto.Security;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Auth
{
    public class RefreshRequest : IRequest<BaseResponse<TokenPair>>
    {
        public string? RefreshToken { get; set; }

        public RefreshRequest(string? refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
