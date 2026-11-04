using Microsoft.OpenApi.Models;

namespace TSP.Api.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Scheme = "bearer",
                        Description = "Enter your JWT access token."
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference()
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "API",
                        Version = "v1",
                    });
                });
            }

            return services;
        }
    }
}
