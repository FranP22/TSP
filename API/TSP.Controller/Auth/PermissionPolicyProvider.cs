using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using TSP.Common.Dto.Security;
using TSP.Common.Enum;

namespace TSP.Controller.Auth
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            const string prefix = "Permission:";

            if (policyName.StartsWith(prefix))
            {
                var permissionName = policyName[prefix.Length..];

                if (!Enum.TryParse<RolePermission>(permissionName, out var permission) || !Enum.IsDefined(permission))
                {
                    return Task.FromResult<AuthorizationPolicy?>(null);
                }

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
