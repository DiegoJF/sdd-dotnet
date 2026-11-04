using RealtorApi.Infrastructure;
using RealtorApi.Infrastructure.Endpoints;
using RealtorApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(typeof(Program).Assembly, builder.Configuration);
builder.Services.AddOpenApi("v1");

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
	await app.MigrateAsync();
}

app.MapInfrastructureEndpoints();

app.Run();

public partial class Program;
