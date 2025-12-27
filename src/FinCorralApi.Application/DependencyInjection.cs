using Microsoft.Extensions.DependencyInjection;

namespace FinCorralApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar validators, handlers, pipelines, MediatR, FluentValidation, etc. aquí.
            // Por ahora no registramos implementaciones concretas (se hace en Infrastructure).
            return services;
        }
    }
}