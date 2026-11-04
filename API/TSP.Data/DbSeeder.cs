using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Db;
using TSP.Common.Enum;
using TSP.Common.Helper;

namespace TSP.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            await CreateDefaultRoles(db);

            await CreateDefaultAdminUser(db);
        }

        private static async Task CreateDefaultRoles(AppDbContext db)
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

            await RemoveInvalidPermissionClaims(db);
        }

        private static async Task RemoveInvalidPermissionClaims(AppDbContext db)
        {
            var validPermissions = Enum.GetNames<RolePermission>();

            await db.RoleClaims
                .Where(x => x.ClaimType == "Permission" && !validPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();
        }

        private static async Task CreateDefaultAdminUser(AppDbContext db)
        {
            var adminRoleId = await db.Roles
                .Where(x => x.IsAdmin && x.Name == "Admin")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (adminRoleId == null)
                throw new InvalidOperationException("Admin role not found.");

            var adminExists = await db.Users
                .Where(x => adminRoleId == x.RoleId)
                .AnyAsync();

            if (adminExists) return;

            var adminEmail = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_EMAIL");

            var adminPassword = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword)) 
                throw new InvalidOperationException("Admin credentials are missing.");

            var adminEmailNormalized = adminEmail.Trim().ToUpperInvariant();

            var adminUser = await db.Users.Where(x => x.EmailNormalized == adminEmailNormalized).FirstOrDefaultAsync();

            if (adminUser != null)
            {
                adminUser.RoleId = adminRoleId;

                await db.SaveChangesAsync();

                return;
            }

            adminUser = new AppUser()
            {
                Email = adminEmail,
                EmailNormalized = adminEmailNormalized,
                RoleId = adminRoleId,
            };

            var adminPasswordHash = SecurityHelper.CreatePasswordHash(adminUser, adminPassword);

            adminUser.PasswordHash = adminPasswordHash;

            db.Users.Add(adminUser);

            await db.SaveChangesAsync();
        }
    }
}
