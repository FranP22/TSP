using MediatR;
using TSP.Common.Dto.Security;
using TSP.Data;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Request.Auth;
using TSP.Services.Service.Interface;

namespace TSP.Mediator.Handler.Auth
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, BaseResponse<TokenPair>>
    {
        private readonly AppDbContext _db;
        private readonly IAuthService _auth;

        public LoginRequestHandler(AppDbContext db, IAuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        public async Task<BaseResponse<TokenPair>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
