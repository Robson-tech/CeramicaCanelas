using CeramicaCanelas.Application.Contracts.Application.Services;
using CeramicaCanelas.Application.Services.SMTPEmail;
using CeramicaCanelas.Application.Services.TokenJwt;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;


namespace CeramicaCanelas.Application.IoC
{
    public static class ApplicationDependencyInjector
    {
        public static IServiceCollection InjectApplicationDependencies(this IServiceCollection services)
        {

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISend, Send>();

            return services;
        }
    }
}
