using Moq;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;
using TSP.Common.Helper;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Test.Tests.Service.Role
{
    [TestFixture]
    class HasPermissionTests : BaseTest
    {
        private RoleService _role;

        private AppRole _role1;
        private AppRole _role2;

        private AppRoleClaim _claim1;
        private AppRoleClaim _claim2;
        private AppRoleClaim _claim3;

        [SetUp]
        public async Task SetUp()
        {
            _role = new RoleService(_db);

            _role1 = new AppRole()
            {
                Name = "Role1",
                NameNormalized = "ROLE1"
            };

            _role2 = new AppRole()
            {
                Name = "Role2",
                NameNormalized = "ROLE2"
            };

            _claim1 = new AppRoleClaim()
            {
                RoleId = _role1.Id,
                ClaimType = "Permission",
                ClaimValue = RolePermission.UserGet.ToString()
            };

            _claim2 = new AppRoleClaim()
            {
                RoleId = _role1.Id,
                ClaimType = "Claim",
                ClaimValue = RolePermission.UserCreate.ToString()
            };

            _claim3 = new AppRoleClaim()
            {
                RoleId = _role2.Id,
                ClaimType = "Permission",
                ClaimValue = RolePermission.UserDelete.ToString()
            };

            _db.Roles.Add(_role1);
            _db.Roles.Add(_role2);
            _db.RoleClaims.Add(_claim1);
            _db.RoleClaims.Add(_claim2);
            _db.RoleClaims.Add(_claim3);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task HasPermission_WhenExists_ReturnsTrue()
        {
            var result = await _role.HasPermissionAsync(_role1.Id, RolePermission.UserGet);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HasPermission_WhenClaimDoesntExist_ReturnsFalse()
        {
            var result = await _role.HasPermissionAsync(_role1.Id, RolePermission.UserUpdate);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasPermission_WhenWrongClaimType_ReturnsFalse()
        {
            var result = await _role.HasPermissionAsync(_role1.Id, RolePermission.UserCreate);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasPermission_WhenDifferentRoleId_ReturnsFalse()
        {
            var result = await _role.HasPermissionAsync(_role1.Id, RolePermission.UserDelete);

            Assert.That(result, Is.False);
        }
    }
}
