using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using System.Security.Cryptography;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;
using TSP.Common.Helper;
using TSP.Common.Options;
using TSP.Services.Service;

namespace TSP.Test.Tests.Service.Token
{
    public class RefreshTokensTests : BaseTest
    {
        private TokenService _token;

        private Mock<IOptions<JwtOptions>> _jwt;

        private AppUser _user;
        private AppRole _role;
        private List<AppRoleClaim> _claims;
        private AppUserSession _userSession;

        private string _refreshToken = "";

        [SetUp]
        public async Task SetUp()
        {
            _jwt = new Mock<IOptions<JwtOptions>>();

            _jwt
                .Setup(x => x.Value)
                .Returns(new JwtOptions
                {
                    SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                    Issuer = "TestIssuer",
                    Audience = "TestAudience",
                    AccessTokenMinutes = 10,
                    RefreshTokenDays = 2
                });

            _token = new TokenService(_db, _jwt.Object);

            _role = new AppRole()
            {
                Name = "Role",
                NameNormalized = "ROLE"
            };

            _claims = new List<AppRoleClaim>()
            {
                new AppRoleClaim()
                {
                    RoleId = _role.Id,
                    ClaimType = "Permission",
                    ClaimValue = RolePermission.UserGet.ToString()
                },
                new AppRoleClaim()
                {
                    RoleId = _role.Id,
                    ClaimType = "Permission",
                    ClaimValue = RolePermission.RoleGet.ToString()
                }
            };

            _user = new AppUser()
            {
                Email = "email@gmail.com",
                EmailNormalized = "EMAIL@GMAIL.COM",
                RoleId = _role.Id
            };

            _refreshToken = SecurityHelper.GenerateRefreshToken();

            var now = DateTime.UtcNow;

            _userSession = new AppUserSession()
            {
                UserId = _user.Id,
                RefreshTokenHash = SecurityHelper.Hash(_refreshToken),
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwt.Object.Value.RefreshTokenDays),
                RevokedAt = null,
                LastRotatedAt = null
            };

            _db.Roles.Add(_role);
            _db.RoleClaims.AddRange(_claims);
            _db.Users.Add(_user);
            _db.UserSessions.Add(_userSession);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task RefreshTokens_WhenSuccess_ReturnsTokenPair()
        {
            var oldHash = _userSession.RefreshTokenHash;

            var result = await _token.RefreshTokensAsync(_refreshToken);

            var session = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .FirstOrDefaultAsync();

            var newRefresh = result?.RefreshToken;

            var oldTokens = await _db.UsedRefreshTokens
                .Where(x => x.SessionId == _userSession.Id)
                .ToListAsync();

            var oldTokenCount = oldTokens.Count;
            var oldToken = oldTokens.FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                
                Assert.That(session, Is.Not.Null);
                Assert.That(session?.LastRotatedAt, Is.Not.Null);

                Assert.That(newRefresh, Is.Not.EqualTo(oldHash));

                Assert.That(oldTokenCount, Is.EqualTo(1));
                Assert.That(oldToken?.TokenHash, Is.EqualTo(oldHash));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenRefreshTokenEmpty_ReturnsNull()
        {
            var result = await _token.RefreshTokensAsync("");

            var session = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .FirstOrDefaultAsync();

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(session, Is.Not.Null);
                Assert.That(session?.LastRotatedAt, Is.Null);
                Assert.That(oldTokens, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenSessionDoesntExist_ReturnsNull()
        {
            var refreshToken = SecurityHelper.GenerateRefreshToken();

            var result = await _token.RefreshTokensAsync(refreshToken);

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(_userSession.LastRotatedAt, Is.Null);
                Assert.That(oldTokens, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenOldTokenUsed_ReturnsNullAndRevoke()
        {
            var firstResult = await _token.RefreshTokensAsync(_refreshToken);

            var secondResult = await _token.RefreshTokensAsync(_refreshToken);

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(secondResult, Is.Null);
                Assert.That(_userSession.RevokedAt, Is.Not.Null);
                Assert.That(_userSession.RevokeReason, Is.Not.Null);
                Assert.That(oldTokens, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenRevokedTokenUsed_ReturnsNull()
        {
            _userSession.RevokedAt = DateTime.UtcNow;
            _userSession.RevokeReason = "";

            var result = await _token.RefreshTokensAsync(_refreshToken);

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(_userSession.LastRotatedAt, Is.Null);
                Assert.That(_userSession.RevokedAt, Is.Not.Null);
                Assert.That(_userSession.RevokeReason, Is.Not.Null);
                Assert.That(oldTokens, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenExpiredTokenUsed_ReturnsNull()
        {
            var expiration = DateTime.UtcNow;
            _userSession.ExpiresAt = expiration;

            var result = await _token.RefreshTokensAsync(_refreshToken);

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(_userSession.LastRotatedAt, Is.Null);
                Assert.That(_userSession.ExpiresAt, Is.LessThanOrEqualTo(expiration));
                Assert.That(oldTokens, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task RefreshTokens_WhenUserIsInactive_ReturnsNull()
        {
            _user.IsActive = false;

            var result = await _token.RefreshTokensAsync(_refreshToken);

            var oldTokens = await _db.UsedRefreshTokens.Where(x => x.SessionId == _userSession.Id).CountAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(_userSession.LastRotatedAt, Is.Null);
                Assert.That(oldTokens, Is.EqualTo(0));
            });
        }
    }
}
