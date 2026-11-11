using Moq;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Helper;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Service.Auth
{
    public class LogoutTests : BaseTest
    {
        private AuthService _auth;

        private Mock<ITokenService> _token;

        private AppUser _user;
        private AppRole _role;

        private AppUserSession _userSession;

        private string _refreshToken;

        [SetUp]
        public async Task SetUp()
        {
            _token = new Mock<ITokenService>();

            _token
                .Setup(x => x.RevokeSessionAsync(It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(() =>
                {
                    _userSession.RevokedAt = DateTime.UtcNow;
                    _userSession.RevokeReason = "Revoked";

                    return Task.CompletedTask;
                });

            _auth = new AuthService(_db, _token.Object);

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

            _user.PasswordHash = SecurityHelper.CreatePasswordHash(_user, Guid.NewGuid().ToString());

            _refreshToken = SecurityHelper.GenerateRefreshToken();

            _userSession = new AppUserSession()
            {
                UserId = _user.Id,
                RefreshTokenHash = SecurityHelper.Hash(_refreshToken)
            };

            _db.Roles.Add(_role);
            _db.Users.Add(_user);
            _db.UserSessions.Add(_userSession);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task Logout_WhenSuccess_SessionRevoked()
        {
            await _auth.LogoutAsync(_refreshToken);

            Assert.That(_userSession.RevokedAt, Is.Not.Null);
        }

        [Test]
        public async Task Logout_WhenIncorrectRefreshToken_SessionNotRevoked()
        {
            var refreshToken = SecurityHelper.GenerateRefreshToken();

            await _auth.LogoutAsync(refreshToken);

            Assert.That(_userSession.RevokedAt, Is.Null);
        }
    }
}
