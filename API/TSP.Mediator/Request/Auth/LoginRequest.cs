using MediatR;
using TSP.Common.Dto.Security;
using TSP.Mediator.Dto.Request.Auth;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Auth
{
    public class LoginRequest : IRequest<BaseResponse<TokenPair>>
    {
        public LoginRequestDto Dto { get; set; }

        public LoginRequest(LoginRequestDto dto)
        {
            Dto = dto;
        }
    }
}
