using Microsoft.Extensions.DependencyInjection;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Infrastructure.Services;

namespace FinCorralApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Registrar implementaciones concretas: repositorios, DbContext, servicios externos, etc.
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();

            // TODO: registrar DbContext (EF Core) con connection string de Azure SQL y Managed Identity / Key Vault
            return services;
        }
    }
}