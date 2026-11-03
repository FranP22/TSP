using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TSP.Controller.Auth;

namespace TSP.Controller.Extensions
{
    public static class PolicyProviderExtension
    {
        public static IServiceCollection AddPolicyProvider(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            return services;
        }
    }
}
