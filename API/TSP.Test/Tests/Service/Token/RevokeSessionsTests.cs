using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;
using TSP.Common.Helper;
using TSP.Common.Options;
using TSP.Services.Service;

namespace TSP.Test.Tests.Service.Token
{
    [TestFixture]
    public class RevokeSessionsTests : BaseTest
    {
        private TokenService _token;

        private Mock<IOptions<JwtOptions>> _jwt;

        private AppUser _user;
        private AppRole _role;
        private List<AppUserSession> _userSessions;

        private List<string> _refreshTokens = new List<string>();

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

            _user = new AppUser()
            {
                Email = "email@gmail.com",
                EmailNormalized = "EMAIL@GMAIL.COM",
                RoleId = _role.Id
            };

            _refreshTokens.Add(SecurityHelper.GenerateRefreshToken());
            _refreshTokens.Add(SecurityHelper.GenerateRefreshToken());
            _refreshTokens.Add(SecurityHelper.GenerateRefreshToken());

            var now = DateTime.UtcNow;

            _userSessions = new List<AppUserSession>()
            {
                new AppUserSession()
                {
                    UserId = _user.Id,
                    RefreshTokenHash = SecurityHelper.Hash(_refreshTokens[0]),
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(_jwt.Object.Value.RefreshTokenDays),
                    RevokedAt = null,
                    LastRotatedAt = null
                },
                new AppUserSession()
                {
                    UserId = _user.Id,
                    RefreshTokenHash = SecurityHelper.Hash(_refreshTokens[1]),
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(_jwt.Object.Value.RefreshTokenDays),
                    RevokedAt = null,
                    LastRotatedAt = null
                },
                new AppUserSession()
                {
                    UserId = _user.Id,
                    RefreshTokenHash = SecurityHelper.Hash(_refreshTokens[2]),
                    CreatedAt = now,
                    ExpiresAt = now.AddDays(_jwt.Object.Value.RefreshTokenDays),
                    RevokedAt = now,
                    RevokeReason = "AlreadyRevoked",
                    LastRotatedAt = null
                }
            };

            _db.Roles.Add(_role);
            _db.Users.Add(_user);
            _db.UserSessions.AddRange(_userSessions);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task RevokeSession_WhenSuccess_RevokeSession()
        {
            var sessionIndex = 0;
            var revokedReason = "Revoked";

            await _token.RevokeSessionAsync(_userSessions[sessionIndex].Id, revokedReason);

            var activeSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt == null)
                .ToListAsync();

            var revokedSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt != null)
                .ToListAsync();

            var revoked = await _db.UserSessions
                .Where(x => x.Id == _userSessions[sessionIndex].Id)
                .FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(revoked, Is.Not.Null);
                Assert.That(revoked?.RevokedAt, Is.Not.Null);
                Assert.That(revoked?.RevokeReason, Is.EqualTo(revokedReason));

                Assert.That(activeSessions.Count(), Is.EqualTo(1));
                Assert.That(revokedSessions.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task RevokeAllSessions_WhenSuccess_RevokeSessions()
        {
            var revokedReason = "Revoked";

            await _token.RevokeAllSessionAsync(_user, revokedReason);

            var activeSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt == null)
                .ToListAsync();

            var revokedSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt != null)
                .ToListAsync();

            var revoked = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokeReason == revokedReason)
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(revoked.Count(), Is.EqualTo(2));

                Assert.That(activeSessions.Count(), Is.EqualTo(0));
                Assert.That(revokedSessions.Count(), Is.EqualTo(_userSessions.Count()));
            });
        }

        [Test]
        public async Task RevokeSession_WhenAlreadyRevoked_NoChanges()
        {
            var sessionIndex = 2;
            var revokedReason = "Revoked";
            var oldRevokeTime = _userSessions[sessionIndex].RevokedAt;

            await _token.RevokeSessionAsync(_userSessions[sessionIndex].Id, revokedReason);

            var activeSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt == null)
                .ToListAsync();

            var revokedSessions = await _db.UserSessions
                .Where(x => x.UserId == _user.Id)
                .Where(x => x.RevokedAt != null)
                .ToListAsync();

            var revoked = await _db.UserSessions
                .Where(x => x.Id == _userSessions[sessionIndex].Id)
                .FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(revoked, Is.Not.Null);
                Assert.That(revoked?.RevokedAt, Is.EqualTo(oldRevokeTime));

                Assert.That(revoked?.RevokeReason, Is.Not.EqualTo(revokedReason));
                Assert.That(revoked?.RevokeReason, Is.EqualTo("AlreadyRevoked"));

                Assert.That(activeSessions.Count(), Is.EqualTo(2));
                Assert.That(revokedSessions.Count(), Is.EqualTo(1));
            });
        }
    }
}
