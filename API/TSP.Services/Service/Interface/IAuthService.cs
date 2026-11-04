using TSP.Common.Dto.Security;

namespace TSP.Services.Service.Interface
{
    public interface IAuthService
    {
        Task<TokenPair?> LoginAsync(string email,  string password);
        Task<TokenPair?> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}
