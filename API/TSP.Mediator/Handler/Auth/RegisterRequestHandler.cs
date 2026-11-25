using MediatR;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Request.Auth;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Auth
{
    public class RegisterRequestHandler : IRequestHandler<RegisterRequest, BaseResponse>
    {
        private readonly IAuthService _auth;

        public RegisterRequestHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public async Task<BaseResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // TODO: change logic for email verification
            var success = await _auth.RegisterAsync(dto.Email, dto.Password);

            return new BaseResponse()
            {
                Success = success,
                Message = success ? "Successfuly registered." : "Failed to register."
            };
        }
    }
}
