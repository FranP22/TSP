using Microsoft.EntityFrameworkCore;
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
