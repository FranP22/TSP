using TSP.Common.Dto.Security;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class AuthService : IAuthService
    {
        public Task<TokenPair?> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
