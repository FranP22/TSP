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

        public Task<bool> HasPermissionAsync(string roleId, RolePermission permission)
        {
            throw new NotImplementedException();
        }
    }
}
