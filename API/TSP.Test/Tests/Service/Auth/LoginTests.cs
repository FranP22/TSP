using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Enum;
using TSP.Common.Helper;
using TSP.Common.Options;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Service.Auth
{
    public class LoginTests : BaseTest
    {
        private AuthService _auth;

        private Mock<ITokenService> _token;

        private AppUser _user;
        private AppRole _role;

        private string _password;

        [SetUp]
        public async Task SetUp()
        {
            _token = new Mock<ITokenService>();

            _token
                .Setup(x => x.GenerateTokensAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new TokenPair()
                {
                    AccessToken = "Access Token",
                    AccessTokenExpiresAt = DateTime.UtcNow,
                    RefreshToken = "Refresh Token",
                    RefreshTokenExpiresAt = DateTime.UtcNow,
                });

            _auth = new AuthService(_db, _token.Object);

            _role = new AppRole()
            {
                Name = "Role",
                NameNormalized = "ROLE"
            };

            _password = Guid.NewGuid().ToString();

            _user = new AppUser()
            {
                Email = "email@gmail.com",
                EmailNormalized = "EMAIL@GMAIL.COM",
                RoleId = _role.Id
            };

            _user.PasswordHash = SecurityHelper.CreatePasswordHash(_user, _password);

            _db.Roles.Add(_role);
            _db.Users.Add(_user);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task Login_WhenSuccess_ReturnsTokenPair()
        {
            var result = await _auth.LoginAsync(_user.Email, _password);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.AccessToken, Is.EqualTo("Access Token"));
                Assert.That(result?.RefreshToken, Is.EqualTo("Refresh Token"));
            });
        }

        [Test]
        public async Task Login_WhenInvalidEmail_ReturnsNull()
        {
            var result = await _auth.LoginAsync("test@gmail.com", _password);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Login_WhenInvalidPassword_ReturnsNull()
        {
            var result = await _auth.LoginAsync(_user.Email, "test_password");

            Assert.That(result, Is.Null);
        }
    }
}
