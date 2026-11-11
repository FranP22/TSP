using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Helper;
using TSP.Common.Options;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Auth.Jwt
{
    [TestFixture]
    public class JwtTests : BaseTest
    {
        private TokenService _token;

        private Mock<IOptions<JwtOptions>> _jwt;

        private AppUser _user;
        private AppRole _role;

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

            _db.Roles.Add(_role);
            _db.Users.Add(_user);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task JwtToken_ShouldBeValid()
        {
            var tokenPair = await _token.GenerateTokensAsync(_user);

            var handler = new JwtSecurityTokenHandler();

            var options = _jwt.Object.Value;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = options.Issuer,

                ValidateAudience = true,
                ValidAudience = options.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(options.SigningKey)
                ),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            var principal = handler.ValidateToken(tokenPair?.AccessToken, validationParameters, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;

            Assert.Multiple(() =>
            {
                Assert.That(principal.Identity!.IsAuthenticated, Is.True);
                Assert.That(jwt.Issuer, Is.EqualTo(options.Issuer));
                Assert.That(jwt.Audiences, Does.Contain(options.Audience));
            });
        }

        [Test]
        public async Task JwtToken_ShouldContainExpectedClaims()
        {
            var tokenPair = await _token.GenerateTokensAsync(_user);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(tokenPair?.AccessToken);

            var claims = jwt.Claims.ToDictionary(
                x => x.Type,
                x => x.Value
            );

            Assert.Multiple(() =>
            {
                Assert.That(claims, Does.ContainKey(JwtRegisteredClaimNames.Sub));
                Assert.That(claims, Does.ContainKey(JwtRegisteredClaimNames.Jti));
                Assert.That(claims, Does.ContainKey(JwtRegisteredClaimNames.Email));
                Assert.That(claims, Does.ContainKey("role"));
                Assert.That(claims, Does.ContainKey("role_id"));
                Assert.That(claims, Does.ContainKey("active"));
                Assert.That(claims, Does.ContainKey("auth_version"));

                Assert.That(
                    claims[JwtRegisteredClaimNames.Sub],
                    Is.EqualTo(_user.Id.ToString())
                );

                Assert.That(
                    claims[JwtRegisteredClaimNames.Jti],
                    Is.Not.Empty
                );

                Assert.That(
                    claims[JwtRegisteredClaimNames.Email],
                    Is.EqualTo(_user.Email)
                );

                Assert.That(
                    claims["role"],
                    Is.EqualTo(_user.Role.Name)
                );

                Assert.That(
                    claims["role_id"],
                    Is.EqualTo(_user.RoleId.ToString())
                );

                Assert.That(
                    claims["active"],
                    Is.EqualTo(_user.IsActive.ToString())
                );

                Assert.That(
                    claims["auth_version"],
                    Is.EqualTo(_user.AuthVersion.ToString())
                );
            });
        }
    }
}
