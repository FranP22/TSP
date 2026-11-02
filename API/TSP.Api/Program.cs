using DotNetEnv;
using TSP.Data;
using TSP.Data.Extensions;
using TSP.Mediator.Extensions;
using TSP.Services.Dependency;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddMediator();

builder.Services.AddDatabase(config);

builder.Services.AddServices();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
