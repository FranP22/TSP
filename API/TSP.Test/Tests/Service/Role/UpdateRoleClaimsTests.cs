using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Enum;
using TSP.Services.Service;

namespace TSP.Test.Tests.Service.Role
{
    public class UpdateRoleClaimsTests : BaseTest
    {
        private RoleService _role;

        private AppRole _existingRole;

        private List<AppRoleClaim> _existingClaims;

        [SetUp]
        public async Task SetUp()
        {
            _role = new RoleService(_db);

            _existingRole = new AppRole()
            {
                Name = "Role",
                NameNormalized = "ROLE"
            };

            _existingClaims = new List<AppRoleClaim>()
            {
                new AppRoleClaim()
                {
                    RoleId = _existingRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = RolePermission.UserGet.ToString()
                },
                new AppRoleClaim()
                {
                    RoleId = _existingRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = RolePermission.UserCreate.ToString()
                }
            };

            _db.Roles.Add(_existingRole);
            _db.RoleClaims.AddRange(_existingClaims);

            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task UpdateRoleClaims_WhenMatchingDefinition_NoChanges()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserGet.ToString(),
                    Enabled = true
                },
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserCreate.ToString(),
                    Enabled = true
                }
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.True);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenEmptyDefinition_NoChanges()
        {
            var definitions = new List<ClaimDefinition>()
            {
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.True);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenExistingClaimDisabled_UpdateClaims()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserCreate.ToString(),
                    Enabled = false
                },
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(1));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.False);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenNonExistingClaimDisabled_NoChanges()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserUpdate.ToString(),
                    Enabled = false
                },
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserUpdate.ToString()), Is.False);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenExistingClaimEnabled_NoChanges()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserGet.ToString(),
                    Enabled = true
                },
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.True);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenNonExistingClaimEnabled_AddClaim()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserUpdate.ToString(),
                    Enabled = true
                },
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(3));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserUpdate.ToString()), Is.True);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenMultipleClaimsUpdated_UpdateClaims()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserUpdate.ToString(),
                    Enabled = true
                },
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserGet.ToString(),
                    Enabled = true
                },
                new ClaimDefinition()
                {
                    Permission = RolePermission.UserCreate.ToString(),
                    Enabled = false
                },
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserGet.ToString()), Is.True);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserCreate.ToString()), Is.False);
                Assert.That(claims.Any(x => x.ClaimValue == RolePermission.UserUpdate.ToString()), Is.True);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenChangingNonExistingClaim_NoChanges()
        {
            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = "Claim",
                    Enabled = true
                }
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var claims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(claims.Count(), Is.EqualTo(2));

                Assert.That(claims.Any(x => x.ClaimValue == "Claim"), Is.False);
            });
        }

        [Test]
        public async Task UpdateRoleClaims_WhenChangingNonPermissionClaim_NoChanges()
        {
            _db.RoleClaims.Add(new AppRoleClaim()
            {
                RoleId = _existingRole.Id,
                ClaimValue = RolePermission.UserGet.ToString(),
                ClaimType = "Claim"
            });

            await _db.SaveChangesAsync();

            var definitions = new List<ClaimDefinition>()
            {
                new ClaimDefinition()
                {
                    Permission = "Claim",
                    Enabled = false
                }
            };

            var result = await _role.UpdateRoleClaimsAsync(_existingRole.Id, definitions);

            var permissionClaims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Permission")
                .ToListAsync();

            var nonPermissionClaims = await _db.RoleClaims
                .Where(x => x.RoleId == _existingRole.Id && x.ClaimType == "Claim")
                .ToListAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                Assert.That(permissionClaims.Count(), Is.EqualTo(2));
                Assert.That(nonPermissionClaims.Count(), Is.EqualTo(1));

                Assert.That(nonPermissionClaims.Any(x => x.ClaimValue == "Claim"), Is.False);
            });
        }
    }
}
