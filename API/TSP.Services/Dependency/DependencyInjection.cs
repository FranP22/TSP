using Microsoft.Extensions.DependencyInjection;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Services.Dependency
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();

            return services;
        }
    }
}
