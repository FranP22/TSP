using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;

namespace TSP.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            await CreateDefaultRoles(db);

            await RemoveInvalidPermissionClaims(db);
        }

        public static async Task CreateDefaultRoles(AppDbContext db)
        {
            var roleNames = new List<string>()
            {
                "Admin",
                "User"
            };

            // Add roles
            foreach (var roleName in roleNames)
            {
                var normalizedName = roleName.Trim().ToUpperInvariant();

                var roleExists = await db.Roles.Where(x => x.NameNormalized == normalizedName).AnyAsync();

                if (roleExists) continue;

                var isAdmin = roleName == "Admin";

                var newRole = new AppRole()
                {
                    Name = roleName,
                    NameNormalized = normalizedName,
                    IsDefault = true,
                    IsAdmin = isAdmin
                };

                db.Roles.Add(newRole);

                await db.SaveChangesAsync();
            }

            // Add missing permissions for admins
            var admins = await db.Roles.Where(x => x.IsAdmin).Include(x => x.Claims).ToListAsync();
            var permissions = Enum.GetNames<RolePermission>().ToHashSet();

            foreach (var admin in admins)
            {
                var currentClaims = admin.Claims.Where(x => x.ClaimType == "Permission").Select(x => x.ClaimValue).ToHashSet();

                var missingClaims = permissions
                    .Where(x => !currentClaims.Contains(x))
                    .Select(x => new AppRoleClaim() 
                    {
                        RoleId = admin.Id,
                        ClaimType = "Permission",
                        ClaimValue = x
                    })
                    .ToList();

                db.RoleClaims.AddRange(missingClaims);
            }

            await db.SaveChangesAsync();
        }

        public static async Task RemoveInvalidPermissionClaims(AppDbContext db)
        {
            var validPermissions = Enum.GetNames<RolePermission>();

            await db.RoleClaims
                .Where(x => x.ClaimType == "Permission" && !validPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();
        }
    }
}
