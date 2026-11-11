using Microsoft.AspNetCore.Authorization;
using Moq;
using System.Security.Claims;
using TSP.Common.Enum;
using TSP.Controller.Auth;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Auth.Permission
{
    [TestFixture]
    public class PermissionHandlerTests
    {
        private PermissionHandler _handler;
        private Mock<IRoleService> _role;

        private static List<RolePermission> _rolePermissions = new List<RolePermission>()
        {
            RolePermission.UserGet,
            RolePermission.UserCreate
        };

        private static IEnumerable<TestCaseData> RoleHasClaims()
        {
            foreach (var permission in _rolePermissions)
            {
                yield return new TestCaseData(permission);
            }
        }

        private static IEnumerable<TestCaseData> RoleDoesntHaveClaims()
        {
            var permissions = Enum.GetValues<RolePermission>().Where(x => !_rolePermissions.Any(y => y == x));

            foreach (var permission in permissions)
            {
                yield return new TestCaseData(permission);
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            _role = new Mock<IRoleService>();

            _role
                .Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<RolePermission>()))
                .ReturnsAsync((string role, RolePermission permission) =>
                {
                    return _rolePermissions.Contains(permission);
                });

            _handler = new PermissionHandler(_role.Object);
        }

        private ClaimsPrincipal CreateUser(string? roleId)
        {
            var claims = new List<Claim>();

            if (roleId != null)
                claims.Add(new Claim("role_id", roleId));

            return new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Test")
            );
        }

        private AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, RolePermission permission)
        {
            var requirement = new PermissionRequirement(permission);

            return new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);
        }

        [TestCaseSource(nameof(RoleHasClaims))]
        [Test]
        public async Task HandleRequirement_WhenUserHasPermission_Succeeds(RolePermission permission)
        {
            var user = CreateUser("1");
            var context = CreateContext(user, permission);

            await _handler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.True);

            _role.Verify(
                x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<RolePermission>()),
                Times.Once);
        }

        [TestCaseSource(nameof(RoleDoesntHaveClaims))]
        [Test]
        public async Task HandleRequirement_WhenUserDoesntHavePermission_Fails(RolePermission permission)
        {
            var user = CreateUser("1");
            var context = CreateContext(user, permission);

            await _handler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.False);

            _role.Verify(
                x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<RolePermission>()),
                Times.Once);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [Test]
        public async Task HandleRequirement_WhenRoleIsEmpty_Fails(string? roleId)
        {
            var user = CreateUser(roleId);
            var context = CreateContext(user, RolePermission.UserGet);

            await _handler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.False);

            _role.Verify(
                x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<RolePermission>()),
                Times.Never);
        }
    }
}
