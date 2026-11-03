using TSP.Common.Enum;

namespace TSP.Services.Service.Interface
{
    public interface IRoleService
    {
        Task<bool> HasPermissionAsync(string roleId, RolePermission permission);
    }
}
