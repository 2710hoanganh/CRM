using Application.Repositories.Base;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register AutoMapper với tất cả Profiles trong Infrastructure assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register IAutoMapper → MappingService
            services.AddScoped<IAutoMapper, MappingService>();
            services.AddScoped<IHashPassword, HashingService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}