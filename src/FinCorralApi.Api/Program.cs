using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FinCorralApi.Application;
using FinCorralApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Añadir servicios
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// TODO: configurar EF Core DbContext con Azure SQL y secretos desde Key Vault
// TODO: configurar Authentication / Authorization, Application Insights, CORS, etc.

var app = builder.Build();

app.MapControllers();

app.Run();