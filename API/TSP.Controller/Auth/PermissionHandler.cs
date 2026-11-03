using Microsoft.AspNetCore.Authorization;
using TSP.Services.Service.Interface;

namespace TSP.Controller.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRoleService _role;

        public PermissionHandler(IRoleService role)
        {
            _role = role;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var roleId = context.User.FindFirst("role_id")?.Value;

            if (string.IsNullOrEmpty(roleId)) return;

            // TODO: Add cache
            var hasPermission = await _role.HasPermissionAsync(roleId, requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
