using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Helper;
using TSP.Common.Options;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _db;
        private readonly JwtOptions _jwtOpt;

        public TokenService(AppDbContext db, IOptions<JwtOptions> jwtOpt)
        {
            _db = db;

            _jwtOpt = jwtOpt.Value;
        }

        public async Task<TokenPair?> GenerateTokensAsync(AppUser user)
        {
            if (!user.IsActive) return null;

            var now = DateTime.UtcNow;

            var refreshToken = SecurityHelper.GenerateRefreshToken();
            var refreshTokenHash = SecurityHelper.Hash(refreshToken);

            var session = new AppUserSession()
            {
                UserId = user.Id,
                RefreshTokenHash = refreshTokenHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwtOpt.RefreshTokenDays)
            };

            _db.UserSessions.Add(session);

            await _db.SaveChangesAsync();

            return CreateTokenResponse(user, session.Id, refreshToken, now);
        }

        public async Task<TokenPair?> RefreshTokensAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return null;

            var now = DateTime.UtcNow;
            var tokenHash = SecurityHelper.Hash(refreshToken);

            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var session = await _db.UserSessions
                .Where(x => x.RefreshTokenHash == tokenHash)
                .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.Claims)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                // Check if already rotated
                var usedToken = await _db.UsedRefreshTokens
                    .Where(x => x.TokenHash == tokenHash)
                    .Include(x => x.UserSession)
                    .FirstOrDefaultAsync();

                if (usedToken != null)
                {
                    var usedSession = usedToken.UserSession;

                    if (usedSession.RevokedAt == null)
                    {
                        usedSession.RevokedAt = now;
                        usedSession.RevokeReason = "Refresh token reused.";

                        await _db.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
            }

            // Validate session
            if (session.RevokedAt != null ||
                session.ExpiresAt <= now ||
                !session.User.IsActive)
            {
                await transaction.CommitAsync();
                return null;
            }

            var oldToken = new AppUsedRefreshTokens()
            {
                SessionId = session.Id,
                TokenHash = tokenHash,
                UsedAt = now
            };
            _db.UsedRefreshTokens.Add(oldToken);

            var newRefreshToken = SecurityHelper.GenerateRefreshToken();
            var newRefreshTokenHash = SecurityHelper.Hash(newRefreshToken);

            session.RefreshTokenHash = newRefreshTokenHash;
            session.LastRotatedAt = now;
            session.ExpiresAt = now.AddDays(_jwtOpt.RefreshTokenDays);

            await _db.SaveChangesAsync();

            return CreateTokenResponse(session.User, session.Id, newRefreshToken, now);
        }

        public async Task RevokeAllSessionAsync(AppUser user, string? reason = null)
        {
            var sessions = await _db.UserSessions
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var session in sessions)
            {
                session.RevokedAt = now;
                session.RevokeReason = reason;
            }

            await _db.SaveChangesAsync();
        }

        public async Task RevokeSessionAsync(string sessionId, string? reason = null)
        {
            var session = await _db.UserSessions
                .Where(x => x.Id == sessionId)
                .FirstOrDefaultAsync();

            if (session == null || session.RevokedAt != null) return;

            session.RevokedAt = DateTime.UtcNow;
            session.RevokeReason = reason;

            await _db.SaveChangesAsync();
        }

        private TokenPair CreateTokenResponse(AppUser user, string sessionId, string refreshToken, DateTime now)
        {
            var accessTokenExpiresAt = now.AddMinutes(_jwtOpt.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),

                new("role", user.Role.Name),
                new("role_id", user.RoleId),
                new("session_id", sessionId),
                new("auth_version", user.AuthVersion.ToString())
            };

            // Add custom role permissions/claims.
            foreach (var roleClaim in user.Role.Claims)
            {
                claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOpt.SigningKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOpt.Issuer,
                audience: _jwtOpt.Audience,
                claims: claims,
                notBefore: now,
                expires: accessTokenExpiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt
            };
        }
    }
}
