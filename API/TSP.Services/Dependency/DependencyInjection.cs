using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TSP.Services.Service;
using TSP.Services.Service.Interface;

namespace TSP.Services.Dependency
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddCacheServices(config);

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRoleService, RoleService>();

            return services;
        }

        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration config)
        {
            var redisConnection = config.GetConnectionString("Redis");

            if (!string.IsNullOrWhiteSpace(redisConnection))
            {
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(redisConnection)
                );

                services.AddSingleton<ICacheService, RedisService>();
            } 
            else
            {
                services.AddMemoryCache();

                services.AddSingleton<ICacheService, CacheService>();
            }

            return services;
        }
    }
}
