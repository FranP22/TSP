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
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private AuthService _auth;

        private Mock<ITokenService> _token;

        private AppUser _user;
        private AppRole _role;

        private const string _password = "TestPassword123!";
        private const string _email = "email@gmail.com";

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

            _user = new AppUser()
            {
                Email = _email,
                EmailNormalized = _email.Trim().ToUpperInvariant(),
                RoleId = _role.Id,
                IsActive = true
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
        public async Task Login_WhenInactive_ReturnsNull()
        {
            _user.IsActive = false;

            var result = await _auth.LoginAsync(_user.Email, _password);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.AccessToken, Is.EqualTo("Access Token"));
                Assert.That(result?.RefreshToken, Is.EqualTo("Refresh Token"));
            });
        }

        [TestCase("", "")]
        [TestCase("test@gmail.com", "")]
        [TestCase("", "Password")]
        [TestCase("test@gmail.com", _password)]
        [TestCase(_email, "Password")]
        [Test]
        public async Task Login_WhenInvalidInformation_ReturnsNull(string email, string password)
        {
            var result = await _auth.LoginAsync("test@gmail.com", _password);

            Assert.That(result, Is.Null);
        }
    }
}
