using Microsoft.Extensions.DependencyInjection;

namespace TSP.Mediator.Extensions
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(MediatorExtension).Assembly);

                //cfg.AddOpenBehavior(typeof())
            });

            return services;
        }
    }
}
