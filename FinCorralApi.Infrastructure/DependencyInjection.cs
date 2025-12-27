using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FinCorralApi.Application.Common.Features;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Infrastructure.Features;
using FinCorralApi.Infrastructure.Repositories;
using FinCorralApi.Infrastructure.Data;


namespace FinCorralApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IFeatureFlagService,Infrastructure.Features.ConfigurationFeatureFlagService>();

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPrestamoRepository, PrestamoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // External services
        // services.AddScoped<IEmailService, EmailService>();

        // Cache
        // services.AddStackExchangeRedisCache(...);

        return services;
    }
}
