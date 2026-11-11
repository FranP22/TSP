using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;
using TSP.Common.Options;
using TSP.Services.Service;

namespace TSP.Test.Tests.Service.Token
{
    [TestFixture]
    public class GenerateTokensTests : BaseTest
    {
        private TokenService _token;

        private Mock<IOptions<JwtOptions>> _jwt;

        private AppUser _user;
        private AppRole _role;
        private List<AppRoleClaim> _claims;

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

            _db.Roles.Add(_role);
            _db.RoleClaims.AddRange(_claims);
            _db.Users.Add(_user);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task GenerateTokens_WhenSuccess_ReturnsTokenPair()
        {
            _user.Role = _role;

            var result = await _token.GenerateTokensAsync(_user);

            var sessions = await _db.UserSessions.Where(x => x.UserId == _user.Id).ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(sessions.Count(), Is.EqualTo(1));
            });
        }


        [Test]
        public async Task GenerateTokens_WhenUserInactive_ReturnsNull()
        {
            _user.IsActive = false;
            _user.Role = _role;

            var result = await _token.GenerateTokensAsync(_user);

            var sessions = await _db.UserSessions.Where(x => x.UserId == _user.Id).ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(sessions.Count(), Is.EqualTo(0));
            });
        }
    }
}
