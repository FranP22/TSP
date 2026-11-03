using TSP.Common.Dto.Db;
using TSP.Common.Dto.Security;
using TSP.Common.Enum;

namespace TSP.Services.Service.Interface
{
    public interface IRoleService
    {
        Task<bool> HasPermissionAsync(string roleId, RolePermission permission);
        Task<AppRole?> CreateRole(string roleName, bool isDefault = false, bool isAdmin = false);
        Task<bool> UpdateRoleClaims(string roleId, List<ClaimDefinition> claims);
    }
}
