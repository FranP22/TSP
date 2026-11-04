using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using TSP.Api.Extensions;
using TSP.Api.Middleware;
using TSP.Common.Options;
using TSP.Data;
using TSP.Data.Extensions;
using TSP.Mediator.Dto.Response;
using TSP.Mediator.Extensions;
using TSP.Services.Dependency;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});
        
builder.Services.AddMediator();

builder.Services.AddDatabase(config);

builder.Services.AddServices();

builder.Services.AddJwt(config);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e =>
                {
                    var message = !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : "Invalid value.";

                    var error = $"{x.Key}: {message}";

                    return error;
                }))
                .ToList();

            var response = new BaseResponse()
            {
                Success = false,
                Message = "Validation failed.",
                Errors = errors
            };

            return new BadRequestObjectResult(response);
        };
    });

// builder.Services.AddCors();

builder.Services.AddSwagger(builder.Environment);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseForwardedHeaders();

if (app.Environment.IsProduction())
{
    //app.UseHsts();
    //app.UseRateLimiter();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
