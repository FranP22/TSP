using Microsoft.AspNetCore.Authorization;
using TSP.Common.Enum;

namespace TSP.Controller.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public RolePermission Permission { get; }

        public PermissionRequirement(RolePermission permission)
        {
            Permission = permission;
        }
    }
}
