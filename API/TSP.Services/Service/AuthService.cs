using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Security;
using TSP.Common.Helper;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _token;

        public AuthService(AppDbContext db, ITokenService token)
        {
            _db = db;
            _token = token;
        }

        public async Task<TokenPair?> LoginAsync(string email, string password)
        {
            var user = await _db.Users
                .Include(x => x.Role)
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var verified = SecurityHelper.VerifyPassword(user, password);

            if (!verified) return null;

            return await _token.GenerateTokensAsync(user);
        }

        public async Task<TokenPair?> RefreshAsync(string refreshToken)
        {
            return await _token.RefreshTokensAsync(refreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var tokenHash = SecurityHelper.Hash(refreshToken);

            var sessionId = await _db.UserSessions.Where(x => x.RefreshTokenHash == tokenHash).Select(x => x.Id).FirstOrDefaultAsync();

            if (sessionId == null) return;

            await _token.RevokeSessionAsync(sessionId, "Logout");

            return;
        }
    }
}
