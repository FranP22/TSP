using MediatR;
using TSP.Mediator.Dto.Request.Auth;
using TSP.Mediator.Dto.Response;

namespace TSP.Mediator.Request.Auth
{
    public class RegisterRequest : IRequest<BaseResponse>
    {
        public RegisterRequestDto Dto { get; set; }

        public RegisterRequest(RegisterRequestDto dto)
        {
            Dto = dto;
        }
    }
}
