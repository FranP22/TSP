using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;

namespace TSP.Services.Service.Interface
{
    public interface IAuthService
    {
        Task<TokenPair?> LoginAsync(string email,  string password);
        Task<bool> RegisterAsync(string email, string password);
        Task<TokenPair?> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task LogoutAllAsync(AppUser user);
        Task<bool> IsUserActive(string userId);
        //Task< IncreaseAuthVersion(Ap);
    }
}
