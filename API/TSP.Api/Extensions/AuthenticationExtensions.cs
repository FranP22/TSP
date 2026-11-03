using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TSP.Common.Options;
using TSP.Controller.Extensions;

namespace TSP.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration config)
        {
            var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration is missing");

            if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
            {
                throw new InvalidOperationException(
                    "JWT signing key is missing.");
            }

            if (Encoding.UTF8.GetByteCount(jwtOptions.SigningKey) < 32)
            {
                throw new InvalidOperationException(
                    "JWT signing key must be at least 32 bytes for HS256.");
            }

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
            {
                throw new InvalidOperationException(
                    "JWT issuer and audience must be configured.");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            services.AddPolicyProvider();

            services.AddAuthorization();

            return services;
        }
    }
}
