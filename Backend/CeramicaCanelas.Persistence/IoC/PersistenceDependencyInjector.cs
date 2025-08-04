using CeramicaCanelas.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CeramicaCanelas.Persistence.IoC;

public static class PersistenceDependencyInjector
{
    public static IServiceCollection InjectPersistenceDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<UtcDateInterceptor>(); // ⬅️ Registra o interceptor

        services.AddDbContext<DefaultContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<UtcDateInterceptor>();
            options
                .UseNpgsql(Environment.GetEnvironmentVariable("POSTGRESQL_CONN"))
                .AddInterceptors(interceptor); // ⬅️ Adiciona o interceptor
        });

        return services;
    }
}
