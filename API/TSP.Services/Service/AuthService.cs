using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Exceptions;
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
            var emailNorm = email.Trim().ToUpperInvariant();

            var user = await _db.Users
                .Include(x => x.Role)
                .Where(x => x.EmailNormalized == emailNorm)
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var verified = SecurityHelper.VerifyPassword(user, password);

            if (!verified) return null;

            return await _token.GenerateTokensAsync(user);
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var emailNorm = email.Trim().ToUpperInvariant();

            var user = await _db.Users
                .Where(x => x.EmailNormalized == emailNorm)
                .AnyAsync();

            if (user) return false;

            var userRole = await _db.Roles
                .Where(x => x.Name == "User")
                .FirstOrDefaultAsync();

            if (userRole == null) throw new NotFoundException("Role not found.");

            var newUser = new AppUser()
            {
                Email = email,
                EmailNormalized = emailNorm,
                RoleId = userRole.Id,
                IsActive = true
            };

            var passwordHash = SecurityHelper.CreatePasswordHash(newUser, password);
            newUser.PasswordHash = passwordHash;

            _db.Users.Add(newUser);

            await _db.SaveChangesAsync();

            return true;
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

        public async Task LogoutAllAsync(AppUser user)
        {
            await _token.RevokeAllSessionAsync(user, "Force Logout");

            return;
        }

        public async Task<bool> IsUserActive(string userId)
        {
            return await _db.Users.AnyAsync(x => x.Id == userId && x.IsActive);
        }
    }
}
