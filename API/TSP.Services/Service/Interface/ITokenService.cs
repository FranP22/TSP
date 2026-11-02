using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;

namespace TSP.Services.Service.Interface
{
    public interface ITokenService
    {
        Task<TokenPair> GenerateTokensAsync(AppUser user);
        Task<TokenPair?> RefreshTokensAsync(string refreshToken);
        Task RevokeSessionAsync(string sessionId, string? reason);
        Task RevokeAllSessionAsync(AppUser user, string? reason);
    }
}
