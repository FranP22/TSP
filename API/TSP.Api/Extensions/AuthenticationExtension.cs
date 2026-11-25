using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TSP.Common.Options;
using TSP.Controller.Extensions;
using TSP.Mediator.Dto.Response;
using TSP.Services.Service.Interface;

namespace TSP.Api.Extensions
{
    public static class AuthenticationExtension
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
                    options.MapInboundClaims = false;

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

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new BaseResponse()
                            {
                                Success = false,
                                Message = "Unauthorized."
                            });
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new BaseResponse
                            {
                                Success = false,
                                Message = "You don't have permission to perform this action."
                            });
                        },

                        OnTokenValidated = async context =>
                        {
                            var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                            var authVersionClaim = context.Principal?.FindFirst("auth_version")?.Value;

                            if (string.IsNullOrEmpty(userId) || !int.TryParse(authVersionClaim, out var tokenVersion))
                            {
                                context.Fail("Invalid token.");

                                return;
                            }

                            var auth = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

                            var isActive = await auth.IsUserActive(userId);

                            if (!isActive)
                            {
                                context.Fail("User is inactive.");
                            }
                        }
                    };
                });

            services.AddPolicyProvider();

            services.AddAuthorization();

            return services;
        }
    }
}
