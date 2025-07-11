using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CeramicaCanelas.Application.Contracts.Infrastructure;
using CeramicaCanelas.Domain.Entities;
using CeramicaCanelas.Infrastructure.Abstractions;
using CeramicaCanelas.Persistence;
using CeramicaCanelas.Application.Contracts.Persistance.Repositories;
using CeramicaCanelas.Persistence.Repositories;

namespace CeramicaCanelas.Infrastructure.IoC;

public static class InfrastructureDependecyInjector {
    /// <summary>
    /// Inject the dependencies of the Infrastructure layer into an
    /// <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to inject the dependencies into
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> with dependencies injected
    /// </returns>
    public static IServiceCollection InjectInfrastructureDependencies(this IServiceCollection services) {
        services.AddDefaultIdentity<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DefaultContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityAbstractor, IdentityAbstractor>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IEmployeesRepository, EmployeesRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
