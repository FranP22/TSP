using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Enum;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;

        public RoleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AppRole?> CreateRoleAsync(string roleName, bool isDefault = false, bool isAdmin = false)
        {
            var normalizedName = roleName.Trim().ToUpperInvariant();

            var roleExists = await _db.Roles.Where(x => x.NameNormalized == normalizedName).AnyAsync();

            if (roleExists)
            {
                return null;
            }

            var role = new AppRole()
            {
                Name = roleName,
                NameNormalized = normalizedName,
                IsDefault = isDefault,
                IsAdmin = isAdmin
            };

            _db.Roles.Add(role);

            await _db.SaveChangesAsync();

            return role;
        }

        public async Task<bool> UpdateRoleClaimsAsync(string roleId, List<ClaimDefinition> claims)
        {
            var role = await _db.Roles
                .Where(x => x.Id == roleId)
                .Include(x => x.Claims)
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return false;
            }

            var projectPermissions = Enum.GetNames<RolePermission>().ToHashSet();

            var validClaims = claims
                .Where(x => projectPermissions.Contains(x.Permission))
                .ToList();

            var permissionsToAdd = validClaims
                .Where(x => x.Enabled)
                .Select(x => x.Permission)
                .ToHashSet();

            var permissionsToRemove = validClaims
                .Where(x => !x.Enabled)
                .Select(x => x.Permission)
                .ToHashSet();

            var existingClaims = await _db.RoleClaims
                .Where(x => x.RoleId == roleId && x.ClaimType == "Permission")
                .ToListAsync();

            var claimsToRemove = existingClaims
                .Where(x => permissionsToRemove.Contains(x.ClaimValue))
                .ToList();

            var existingPermissions = existingClaims
                .Select(x => x.ClaimValue)
                .ToHashSet();

            var claimsToAdd = permissionsToAdd
                .Except(existingPermissions)
                .Select(permission => new AppRoleClaim()
                {
                    RoleId = roleId,
                    ClaimType = "Permission",
                    ClaimValue = permission
                })
                .ToList();

            _db.RoleClaims.RemoveRange(claimsToRemove);
            _db.RoleClaims.AddRange(claimsToAdd);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HasPermissionAsync(string roleId, RolePermission permission)
        {
            var perm = permission.ToString();

            return await _db.RoleClaims.AnyAsync(x =>
                x.RoleId == roleId &&
                x.ClaimType == "Permission" &&
                x.ClaimValue == perm);
        }
    }
}
